using System;
using System.Collections.Generic;
using BebeFlo.Utils;

namespace BebeFlo.Sensors.X2Protocol
{
	public class AsyncFlowSampler : AsyncSamplerBase<FlowRecord>
	{
		public AsyncFlowSampler(X2Controller ctrl, int samplingFrequency, double callbackFrequency, Func<IList<FlowRecord>, bool> dataCb, ExBackgroundWorker.AsyncErrorOccurredHandler errorCb) : base(delegate(Func<IList<FlowRecord>, bool> cb)
		{
			ctrl.FlowSampling(samplingFrequency, callbackFrequency, (IList<FlowRecord> vals) => cb(vals));
		}, dataCb, errorCb)
		{
			if (ctrl == null)
			{
				throw new ArgumentNullException("ctrl");
			}
			if (dataCb == null)
			{
				throw new ArgumentNullException("dataCb");
			}
		}
	}
}
