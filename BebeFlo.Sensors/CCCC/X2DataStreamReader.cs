using System;
using System.Globalization;
using log4net;

namespace BebeFlo.Sensors.X2Protocol
{
    public class X2DataStreamReader
    {
        public X2DataStreamReader(IX2Port serial)
        {
            if (serial == null)
            {
                throw new ArgumentNullException("serial");
            }
            this._serial = serial;
        }

        public void SetValueCallback(X2ChannelNumber channel, X2DataStreamReader.ValueCallback cb)
        {
            this._valueCallbacks[(int)channel] = cb;
        }

        public void SetSpecialErrorHandler(Func<X2ProtocolException, bool> handler)
        {
            this._specialErrorHandler = handler;
        }

        public void Run()
        {
            byte[] array = new byte[4];//每个byte为8位
            this._serial.Read(array, 0, 2);//读取2个字节
            byte b = (byte)(array[0] >> 4);//高四位为通道值
            int num = ((int)(array[0] & 15) << 8) + (int)array[1];//第四位设为1，扩展到16位再左移动8位得到高8位，获得的值高8位+低8位
            switch (b)//通道值
            {
                //TransitTime1
                case 13:
                //TransitTime2
                case 14:
                    this._serial.Read(array, 2, 2);
                    num = (num << 16) + ((int)array[2] << 8) + (int)array[3];
                    X2DataStreamReader._log.DebugFormat("Data received for Transit-Channel (4 bytes): buffer=0x{0:X2}{1:X2}{2:X2}{3:X2} [channel={4} (0x{4:X2}), value={5} (0x{5:X8})]", new object[]
                    {
                    array[0],
                    array[1],
                    array[2],
                    array[3],
                    b,
                    num
                    });
                    if (this._valueCallbacks[(int)b] == null)
                    {
                        throw new X2ProtocolException(string.Format(CultureInfo.InvariantCulture, "no callback registered for channel {0}", new object[]
                        {
                        b
                        }));
                    }
                    this._valueCallbacks[(int)b](num);
                    this._runningChecksum += ((int)array[0] << 8) + (int)array[1];
                    this._runningChecksum += ((int)array[2] << 8) + (int)array[3];
                    return;
                //Control
                case 15:
                    {
                        X2DataStreamReader._log.DebugFormat("Data received for Control-Channel (2 bytes): buffer=0x{0:X2}{1:X2} [channel={2} (0x{2:X2}), indicator={3} (0x{3:X2}), value={4} (0x{4:X2})]", new object[]
                        {
                    array[0],
                    array[1],
                    b,
                    num >> 8,
                    num & 255
                        });
                        int num2 = num >> 8;
                        if (num2 == 4)
                        {
                            this.HandleStreamChecksum(num & 255);
                            return;
                        }
                        if (num2 != 8)
                        {
                            throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, "handler for control channel indicator 0x{0:X2} (with value 0x{1:X2}) is not implemented.", new object[]
                            {
                        num >> 8,
                        num & 255
                            }));
                        }
                        X2DataStreamReader.HandleStreamError(num & 255, this._specialErrorHandler);
                        this._runningChecksumSynced = false;
                        return;
                    }
                    //other
                default:
                    X2DataStreamReader._log.DebugFormat("Data received for Default-Channel (2 bytes): buffer=0x{0:X2}{1:X2} [channel={2} (0x{2:X2}), value={3} (0x{3:X4})]", new object[]
                    {
                    array[0],
                    array[1],
                    b,
                    num
                    });
                    if (this._valueCallbacks[(int)b] == null)
                    {
                        throw new X2ProtocolException(string.Format(CultureInfo.InvariantCulture, "no callback registered for channel {0}", new object[]
                        {
                        b
                        }));
                    }
                    this._valueCallbacks[(int)b](num);
                    //校验码
                    this._runningChecksum += ((int)array[0] << 8) + (int)array[1];
                    return;
            }
        }

        private void HandleStreamChecksum(int val)
        {
            int num = (this._runningChecksum & 255) + (this._runningChecksum >> 8 & 255) & 255;
            if (this._runningChecksumSynced && val != num)
            {
                throw new X2ProtocolException(string.Format("X2 data streaming crc mismatch error. (read=0x{0:x2}, expected=0x{1:x2})", val, num));
            }
            this._runningChecksum = 0;
            this._runningChecksumSynced = true;
        }

        private static void HandleStreamError(int val, Func<X2ProtocolException, bool> specialHandler)
        {
            X2StreamStatus x2StreamStatus = (X2StreamStatus)(val & 31);
            int num = val >> 5 & 3;
            bool flag = (val & 128) != 0;
            X2ProtocolException ex = new X2ProtocolException(string.Format("X2 data streaming error. (type='{0}', port={1}, code={2})", flag ? "Sp-AS" : "Sp-X", num, x2StreamStatus));
            ex.ErrorStatus = x2StreamStatus;
            if (specialHandler == null || specialHandler(ex))
            {
                throw ex;
            }
        }

        private static readonly ILog _log = LogManager.GetLogger(typeof(X2DataStreamReader));

        private readonly IX2Port _serial;

        private int _runningChecksum;

        private bool _runningChecksumSynced;

        private Func<X2ProtocolException, bool> _specialErrorHandler;

        private readonly X2DataStreamReader.ValueCallback[] _valueCallbacks = new X2DataStreamReader.ValueCallback[16];

        public delegate void ValueCallback(int value);
    }
}
