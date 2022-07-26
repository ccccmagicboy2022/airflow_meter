#include "spi.hpp"
#include "sys.h"
#include "sys.hpp"

extern DWTDelay dwt;
uint8_t Intn_flag = 0;
float RT_PT1000[1001] =   // 表格中阻值以0.1℃为1步，从0℃到100℃，共计1001个点
{
	1000.000, 1000.391, 1000.782, 1001.172, 1001.563, 1001.954, 1002.345, 1002.736, 1003.126, 1003.517, // 0℃ ~ 0.9℃
	1003.908, 1004.298, 1004.689, 1005.080, 1005.470, 1005.861, 1006.252, 1006.642, 1007.033, 1007.424, // 1℃ ~ 1.9℃
	1007.814, 1008.205, 1008.595, 1008.986, 1009.377, 1009.767, 1010.158, 1010.548, 1010.939, 1011.329, // 2℃ ~ 2.9℃
	1011.720, 1012.110, 1012.501, 1012.891, 1013.282, 1013.672, 1014.062, 1014.453, 1014.843, 1015.234, // 3℃ ~ 3.9℃
	1015.624, 1016.014, 1016.405, 1016.795, 1017.185, 1017.576, 1017.966, 1018.356, 1018.747, 1019.137, // 4℃ ~ 4.9℃
	1019.527, 1019.917, 1020.308, 1020.698, 1021.088, 1021.478, 1021.868, 1022.259, 1022.649, 1023.039, // 5℃ ~ 5.9℃
	1023.429, 1023.819, 1024.209, 1024.599, 1024.989, 1025.380, 1025.770, 1026.160, 1026.550, 1026.940, // 6℃ ~ 6.9℃
	1027.330, 1027.720, 1028.110, 1028.500, 1028.890, 1029.280, 1029.670, 1030.060, 1030.450, 1030.840, // 7℃ ~ 7.9℃
	1031.229, 1031.619, 1032.009, 1032.399, 1032.789, 1033.179, 1033.569, 1033.958, 1034.348, 1034.738, // 8℃~ 8.9℃
	1035.128, 1035.518, 1035.907, 1036.297, 1036.687, 1037.077, 1037.466, 1037.856, 1038.246, 1038.636, // 9℃ ~ 9.9℃
	1039.025, 1039.415, 1039.805, 1040.194, 1040.584, 1040.973, 1041.363, 1041.753, 1042.142, 1042.532, // 10℃ ~ 10.9℃
	1042.921, 1043.311, 1043.701, 1044.090, 1044.480, 1044.869, 1045.259, 1045.648, 1046.038, 1046.427, // 11℃ ~ 11.9℃
	1046.816, 1047.206, 1047.595, 1047.985, 1048.374, 1048.764, 1049.153, 1049.542, 1049.932, 1050.321, // 12℃ ~ 12.9℃
	1050.710, 1051.100, 1051.489, 1051.878, 1052.268, 1052.657, 1053.046, 1053.435, 1053.825, 1054.214, // 13℃ ~ 13.9℃
	1054.603, 1054.992, 1055.381, 1055.771, 1056.160, 1056.549, 1056.938, 1057.327, 1057.716, 1058.105, // 14℃ ~ 14.9℃
	1058.495, 1058.884, 1059.273, 1059.662, 1060.051, 1060.440, 1060.829, 1061.218, 1061.607, 1061.996, // 15℃ ~ 15.9℃
	1062.385, 1062.774, 1063.163, 1063.552, 1063.941, 1064.330, 1064.719, 1065.108, 1065.496, 1065.885, // 16℃ ~ 16.9℃
	1066.274, 1066.663, 1067.052, 1067.441, 1067.830, 1068.218, 1068.607, 1068.996, 1069.385, 1069.774, // 17℃ ~ 17.9℃
	1070.162, 1070.551, 1070.940, 1071.328, 1071.717, 1072.106, 1072.495, 1072.883, 1073.272, 1073.661, // 18℃ ~ 18.9℃
	1074.049, 1074.438, 1074.826, 1075.215, 1075.604, 1075.992, 1076.381, 1076.769, 1077.158, 1077.546, // 19℃ ~ 19.9℃
	1077.935, 1078.324, 1078.712, 1079.101, 1079.489, 1079.877, 1080.266, 1080.654, 1081.043, 1081.431, // 20℃ ~ 20.9℃
	1081.820, 1082.208, 1082.596, 1082.985, 1083.373, 1083.762, 1084.150, 1084.538, 1084.926, 1085.315, // 21℃ ~ 21.9℃
	1085.703, 1086.091, 1086.480, 1086.868, 1087.256, 1087.644, 1088.033, 1088.421, 1088.809, 1089.197, // 22℃ ~ 22.9℃
	1089.585, 1089.974, 1090.362, 1090.750, 1091.138, 1091.526, 1091.914, 1092.302, 1092.690, 1093.078, // 23℃ ~ 23.9℃
	1093.467, 1093.855, 1094.243, 1094.631, 1095.019, 1095.407, 1095.795, 1096.183, 1096.571, 1096.959, // 24℃ ~ 24.9℃
	1097.347, 1097.734, 1098.122, 1098.510, 1098.898, 1099.286, 1099.674, 1100.062, 1100.450, 1100.838, // 25℃ ~ 25.9℃
	1101.225, 1101.613, 1102.001, 1102.389, 1102.777, 1103.164, 1103.552, 1103.940, 1104.328, 1104.715, // 26℃ ~ 26.9℃
	1105.103, 1105.491, 1105.879, 1106.266, 1106.654, 1107.042, 1107.429, 1107.817, 1108.204, 1108.592, // 27℃ ~ 27.9℃
	1108.980, 1109.367, 1109.755, 1110.142, 1110.530, 1110.917, 1111.305, 1111.693, 1112.080, 1112.468, // 28℃ ~ 28.9℃
	1112.855, 1113.242, 1113.630, 1114.017, 1114.405, 1114.792, 1115.180, 1115.567, 1115.954, 1116.342, // 29℃ ~ 29.9℃
	1116.729, 1117.117, 1117.504, 1117.891, 1118.279, 1118.666, 1119.053, 1119.441, 1119.828, 1120.215, // 30℃ ~ 30.9℃
	1120.602, 1120.990, 1121.377, 1121.764, 1122.151, 1122.538, 1122.926, 1123.313, 1123.700, 1124.087, 
	1124.474, 1124.861, 1125.248, 1125.636, 1126.023, 1126.410, 1126.797, 1127.184, 1127.571, 1127.958, 
	1128.345, 1128.732, 1129.119, 1129.506, 1129.893, 1130.280, 1130.667, 1131.054, 1131.441, 1131.828, 
	1132.215, 1132.602, 1132.988, 1133.375, 1133.762, 1134.149, 1134.536, 1134.923, 1135.309, 1135.696, 
	1136.083, 1136.470, 1136.857, 1137.243, 1137.630, 1138.017, 1138.404, 1138.790, 1139.177, 1139.564, 
	1139.950, 1140.337, 1140.724, 1141.110, 1141.497, 1141.884, 1142.270, 1142.657, 1143.043, 1143.430, 
	1143.817, 1144.203, 1144.590, 1144.976, 1145.363, 1145.749, 1146.136, 1146.522, 1146.909, 1147.295, 
	1147.681, 1148.068, 1148.454, 1148.841, 1149.227, 1149.614, 1150.000, 1150.386, 1150.773, 1151.159, 
	1151.545, 1151.932, 1152.318, 1152.704, 1153.091, 1153.477, 1153.863, 1154.249, 1154.636, 1155.022, 
	1155.408, 1155.794, 1156.180, 1156.567, 1156.953, 1157.339, 1157.725, 1158.111, 1158.497, 1158.883,  
	1159.270, 1159.656, 1160.042, 1160.428, 1160.814, 1161.200, 1161.586, 1161.972, 1162.358, 1162.744,  
	1163.130, 1163.516, 1163.902, 1164.288, 1164.674, 1165.060, 1165.446, 1165.831, 1166.217, 1166.603,  
	1166.989, 1167.375, 1167.761, 1168.147, 1168.532, 1168.918, 1169.304, 1169.690, 1170.076, 1170.461,  
	1170.847, 1171.233, 1171.619, 1172.004, 1172.390, 1172.776, 1173.161, 1173.547, 1173.933, 1174.318,  
	1174.704, 1175.090, 1175.475, 1175.861, 1176.247, 1176.632, 1177.018, 1177.403, 1177.789, 1178.174,  
	1178.560, 1178.945, 1179.331, 1179.716, 1180.102, 1180.487, 1180.873, 1181.258, 1181.644, 1182.029,  
	1182.414, 1182.800, 1183.185, 1183.571, 1183.956, 1184.341, 1184.727, 1185.112, 1185.497, 1185.883,  
	1186.268, 1186.653, 1187.038, 1187.424, 1187.809, 1188.194, 1188.579, 1188.965, 1189.350, 1189.735,  
	1190.120, 1190.505, 1190.890, 1191.276, 1191.661, 1192.046, 1192.431, 1192.816, 1193.201, 1193.586,  
	1193.971, 1194.356, 1194.741, 1195.126, 1195.511, 1195.896, 1196.281, 1196.666, 1197.051, 1197.436,  
	1197.821, 1198.206, 1198.591, 1198.976, 1199.361, 1199.746, 1200.131, 1200.516, 1200.900, 1201.285,  
	1201.670, 1202.055, 1202.440, 1202.824, 1203.209, 1203.594, 1203.979, 1204.364, 1204.748, 1205.133,  
	1205.518, 1205.902, 1206.287, 1206.672, 1207.056, 1207.441, 1207.826, 1208.210, 1208.595, 1208.980,  
	1209.364, 1209.749, 1210.133, 1210.518, 1210.902, 1211.287, 1211.672, 1212.056, 1212.441, 1212.825,  
	1213.210, 1213.594, 1213.978, 1214.363, 1214.747, 1215.132, 1215.516, 1215.901, 1216.285, 1216.669,  
	1217.054, 1217.438, 1217.822, 1218.207, 1218.591, 1218.975, 1219.360, 1219.744, 1220.128, 1220.513,  
	1220.897, 1221.281, 1221.665, 1222.049, 1222.434, 1222.818, 1223.202, 1223.586, 1223.970, 1224.355,  
	1224.739, 1225.123, 1225.507, 1225.891, 1226.275, 1226.659, 1227.043, 1227.427, 1227.811, 1228.195,  
	1228.579, 1228.963, 1229.347, 1229.731, 1230.115, 1230.499, 1230.883, 1231.267, 1231.651, 1232.035,  
	1232.419, 1232.803, 1233.187, 1233.571, 1233.955, 1234.338, 1234.722, 1235.106, 1235.490, 1235.874,  
	1236.257, 1236.641, 1237.025, 1237.409, 1237.792, 1238.176, 1238.560, 1238.944, 1239.327, 1239.711,  
	1240.095, 1240.478, 1240.862, 1241.246, 1241.629, 1242.013, 1242.396, 1242.780, 1243.164, 1243.547,  
	1243.931, 1244.314, 1244.698, 1245.081, 1245.465, 1245.848, 1246.232, 1246.615, 1246.999, 1247.382,  
	1247.766, 1248.149, 1248.533, 1248.916, 1249.299, 1249.683, 1250.066, 1250.450, 1250.833, 1251.216,  
	1251.600, 1251.983, 1252.366, 1252.749, 1253.133, 1253.516, 1253.899, 1254.283, 1254.666, 1255.049,  
	1255.432, 1255.815, 1256.199, 1256.582, 1256.965, 1257.348, 1257.731, 1258.114, 1258.497, 1258.881,  
	1259.264, 1259.647, 1260.030, 1260.413, 1260.796, 1261.179, 1261.562, 1261.945, 1262.328, 1262.711,  
	1263.094, 1263.477, 1263.860, 1264.243, 1264.626, 1265.009, 1265.392, 1265.775, 1266.157, 1266.540,  
	1266.923, 1267.306, 1267.689, 1268.072, 1268.455, 1268.837, 1269.220, 1269.603, 1269.986, 1270.368,  
	1270.751, 1271.134, 1271.517, 1271.899, 1272.282, 1272.665, 1273.048, 1273.430, 1273.813, 1274.195,  
	1274.578, 1274.961, 1275.343, 1275.726, 1276.109, 1276.491, 1276.874, 1277.256, 1277.639, 1278.021,  
	1278.404, 1278.786, 1279.169, 1279.551, 1279.934, 1280.316, 1280.699, 1281.081, 1281.464, 1281.846,  
	1282.228, 1282.611, 1282.993, 1283.376, 1283.758, 1284.140, 1284.523, 1284.905, 1285.287, 1285.670,  
	1286.052, 1286.434, 1286.816, 1287.199, 1287.581, 1287.963, 1288.345, 1288.728, 1289.110, 1289.492,  
	1289.874, 1290.256, 1290.638, 1291.021, 1291.403, 1291.785, 1292.167, 1292.549, 1292.931, 1293.313,  
	1293.695, 1294.077, 1294.459, 1294.841, 1295.223, 1295.605, 1295.987, 1296.369, 1296.751, 1297.133,  
	1297.515, 1297.897, 1298.279, 1298.661, 1299.043, 1299.425, 1299.807, 1300.188, 1300.570, 1300.952,  
	1301.334, 1301.716, 1302.098, 1302.479, 1302.861, 1303.243, 1303.625, 1304.006, 1304.388, 1304.770,  
	1305.152, 1305.533, 1305.915, 1306.297, 1306.678, 1307.060, 1307.442, 1307.823, 1308.205, 1308.586,  
	1308.968, 1309.350, 1309.731, 1310.113, 1310.494, 1310.876, 1311.257, 1311.639, 1312.020, 1312.402,  
	1312.783, 1313.165, 1313.546, 1313.928, 1314.309, 1314.691, 1315.072, 1315.453, 1315.835, 1316.216,  
	1316.597, 1316.979, 1317.360, 1317.742, 1318.123, 1318.504, 1318.885, 1319.267, 1319.648, 1320.029,  
	1320.411, 1320.792, 1321.173, 1321.554, 1321.935, 1322.317, 1322.698, 1323.079, 1323.460, 1323.841,  
	1324.222, 1324.603, 1324.985, 1325.366, 1325.747, 1326.128, 1326.509, 1326.890, 1327.271, 1327.652,  
	1328.033, 1328.414, 1328.795, 1329.176, 1329.557, 1329.938, 1330.319, 1330.700, 1331.081, 1331.462,  
	1331.843, 1332.224, 1332.604, 1332.985, 1333.366, 1333.747, 1334.128, 1334.509, 1334.889, 1335.270,  
	1335.651, 1336.032, 1336.413, 1336.793, 1337.174, 1337.555, 1337.935, 1338.316, 1338.697, 1339.078,  
	1339.458, 1339.839, 1340.220, 1340.600, 1340.981, 1341.361, 1341.742, 1342.123, 1342.503, 1342.884,  
	1343.264, 1343.645, 1344.025, 1344.406, 1344.786, 1345.167, 1345.547, 1345.928, 1346.308, 1346.689,  
	1347.069, 1347.450, 1347.830, 1348.211, 1348.591, 1348.971, 1349.352, 1349.732, 1350.112, 1350.493,  
	1350.873, 1351.253, 1351.634, 1352.014, 1352.394, 1352.774, 1353.155, 1353.535, 1353.915, 1354.295,  
	1354.676, 1355.056, 1355.436, 1355.816, 1356.196, 1356.577, 1356.957, 1357.337, 1357.717, 1358.097,  
	1358.477, 1358.857, 1359.237, 1359.617, 1359.997, 1360.377, 1360.757, 1361.137, 1361.517, 1361.897,  
	1362.277, 1362.657, 1363.037, 1363.417, 1363.797, 1364.177, 1364.557, 1364.937, 1365.317, 1365.697,  
	1366.077, 1366.456, 1366.836, 1367.216, 1367.596, 1367.976, 1368.355, 1368.735, 1369.115, 1369.495,  
	1369.875, 1370.254, 1370.634, 1371.014, 1371.393, 1371.773, 1372.153, 1372.532, 1372.912, 1373.292,  // 96℃ ~ 96.9℃
	1373.671, 1374.051, 1374.431, 1374.810, 1375.190, 1375.569, 1375.949, 1376.329, 1376.708, 1377.088,  // 97℃ ~ 97.9℃
	1377.467, 1377.847, 1378.226, 1378.606, 1378.985, 1379.365, 1379.744, 1380.123, 1380.503, 1380.882,  // 98℃ ~ 98.9℃
	1381.262, 1381.641, 1382.020, 1382.400, 1382.779, 1383.158, 1383.538, 1383.917, 1384.296, 1384.676,  // 99℃ ~ 99.9℃
	1385.055 	//100℃
};

Spi::Spi()
{
    init_int();
    init_pin();
    init_spi();
    init_ss_pin();
    init_rst_pin();
    init_int_pin();
    
    m_status = 0x0000;
}

Spi::~Spi()
{
    //
}

void Spi::init_int_pin()
{
    spi_int.init_pin(GPIOA, GPIO_PIN_9);
    spi_int.init_irq();
}

void Spi::init_rst_pin()
{
    spi_rstn.init_pin(GPIOA, GPIO_PIN_10);
    spi_rstn.high();
}

void Spi::init_pin()
{
    GPIO_InitType GPIO_InitStructure;

    GPIO_InitStructure.Pin        = GPIO_PIN_5 | GPIO_PIN_7;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode  = GPIO_Mode_AF_PP;
    GPIO_InitPeripheral(GPIOA, &GPIO_InitStructure);
    
    GPIO_InitStructure.Pin       = GPIO_PIN_6;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IPU;
    GPIO_InitPeripheral(GPIOA, &GPIO_InitStructure);
    
}

void Spi::init_spi()
{
    SPI_InitType SPI_InitStructure;
    
    SPI_InitStructure.DataDirection = SPI_DIR_DOUBLELINE_FULLDUPLEX;
    SPI_InitStructure.SpiMode       = SPI_MODE_MASTER;
    SPI_InitStructure.DataLen       = SPI_DATA_SIZE_8BITS;
    SPI_InitStructure.CLKPOL        = SPI_CLKPOL_LOW;
    SPI_InitStructure.CLKPHA        = SPI_CLKPHA_SECOND_EDGE;
    SPI_InitStructure.NSS           = SPI_NSS_HARD;
    SPI_InitStructure.BaudRatePres  = SPI_BR_PRESCALER_16;   //  64/16=4MHz
    //SPI_InitStructure.BaudRatePres  = SPI_BR_PRESCALER_8;   //  64/8=8MHz
    SPI_InitStructure.FirstBit      = SPI_FB_MSB;       //MSB first
    SPI_InitStructure.CRCPoly       = 7;
    
    SPI_Init(SPI1, &SPI_InitStructure);
    
    //SPI_I2S_EnableInt(SPI1, SPI_I2S_INT_TE, ENABLE);   //send irq
    //SPI_I2S_EnableInt(SPI1, SPI_I2S_INT_RNE, ENABLE);   //rev irq
    
    SPI_SSOutputEnable(SPI1, ENABLE);
    SPI_Enable(SPI1, ENABLE);
}

void Spi::init_int()
{
    NVIC_InitType NVIC_InitStructure;

    NVIC_PriorityGroupConfig(NVIC_PriorityGroup_1);

    NVIC_InitStructure.NVIC_IRQChannel                   = SPI1_IRQn;
    NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 1;
    NVIC_InitStructure.NVIC_IRQChannelSubPriority        = 2;
    NVIC_InitStructure.NVIC_IRQChannelCmd                = ENABLE;
    NVIC_Init(&NVIC_InitStructure);
}

void Spi::write8(uint8_t wbuf8)
{
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_TE_FLAG) == RESET)
        ;
    SPI_I2S_TransmitData(SPI1, wbuf8);
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_BUSY_FLAG) == SET)
        ;
}

uint8_t Spi::read8()
{
    uint16_t result = 0x00;
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_TE_FLAG) == RESET)
        ;
    SPI_I2S_TransmitData(SPI1, 0xAA);
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_RNE_FLAG) == RESET)
        ;
    result = SPI_I2S_ReceiveData(SPI1); //dummy
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_RNE_FLAG) == RESET)
        ;
    result = SPI_I2S_ReceiveData(SPI1);
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_TE_FLAG) == RESET)
        ;
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_BUSY_FLAG) == SET)
        ;
    
    return (uint8_t)result;
}

uint16_t Spi::read16()
{
    uint16_t result0 = 0x00;
    uint16_t result1 = 0x00;
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_TE_FLAG) == RESET)
        ;
    SPI_I2S_TransmitData(SPI1, 0xAA);
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_RNE_FLAG) == RESET)
        ;
    result0 = SPI_I2S_ReceiveData(SPI1); //dummy
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_RNE_FLAG) == RESET)
        ;
    result0 = SPI_I2S_ReceiveData(SPI1); //byte0
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_TE_FLAG) == RESET)
        ;
    SPI_I2S_TransmitData(SPI1, 0xAA); 

    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_RNE_FLAG) == RESET)
        ;
    result1 = SPI_I2S_ReceiveData(SPI1); //byte1    
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_TE_FLAG) == RESET)
        ;
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_BUSY_FLAG) == SET)
        ;
    
    return result0*0x100 + result1;
}

uint32_t Spi::read32()
{
    uint16_t result0 = 0x00;
    uint16_t result1 = 0x00;
    uint16_t result2 = 0x00;
    uint16_t result3 = 0x00;
    uint32_t result = 0x00;
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_TE_FLAG) == RESET)
        ;
    SPI_I2S_TransmitData(SPI1, 0xAA);
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_RNE_FLAG) == RESET)
        ;
    result0 = SPI_I2S_ReceiveData(SPI1); //dummy
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_RNE_FLAG) == RESET)
        ;
    result0 = SPI_I2S_ReceiveData(SPI1); //byte0
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_TE_FLAG) == RESET)
        ;
    SPI_I2S_TransmitData(SPI1, 0xAA); 

    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_RNE_FLAG) == RESET)
        ;
    result1 = SPI_I2S_ReceiveData(SPI1); //byte1    
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_TE_FLAG) == RESET)
        ;
    SPI_I2S_TransmitData(SPI1, 0xAA); 

    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_RNE_FLAG) == RESET)
        ;
    result2 = SPI_I2S_ReceiveData(SPI1); //byte2    
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_TE_FLAG) == RESET)
        ;
    SPI_I2S_TransmitData(SPI1, 0xAA); 

    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_RNE_FLAG) == RESET)
        ;
    result3 = SPI_I2S_ReceiveData(SPI1); //byte3    
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_TE_FLAG) == RESET)
        ;
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_BUSY_FLAG) == SET)
        ;
    
    result = result0*0x1000000 + result1*0x10000 + result2*0x100 + result3;
    return result;
}

void Spi::init_ss_pin()
{
    spi_ss.init_pin(GPIOA, GPIO_PIN_4);
    spi_ss.high();
}

void Spi::Write_Reg(uint8_t RegNum, uint32_t RegData)
{
    spi_ss.low();
    
    write8(RegNum);
    
    write8((0xFF000000 & RegData)>>24);
    write8((0x00FF0000 & RegData)>>16);
    write8((0x0000FF00 & RegData)>>8);
    write8(0x000000FF & RegData);
    
    spi_ss.high();
}

void Spi::Write_Order(uint8_t Order)
{
    spi_ss.low();
    write8(Order);
    spi_ss.high();
}

uint8_t Spi::Read_REG0_L()
{
    uint8_t ReadData = 0;
    
    spi_ss.low();
	write8(READ_COMM_REG);
	ReadData = read8();
    spi_ss.high();
    
	return ReadData;
}

uint16_t Spi::Read_STAT()
{
    uint16_t ReadData = 0;
    
    spi_ss.low();
	write8(READ_STATUS_REG);
	ReadData = read16();
    spi_ss.high();
    
	return ReadData;
}

uint32_t Spi::Read_Reg(uint8_t RegNum)
{
    uint32_t ReadData = 0;
    
    spi_ss.low();
	write8(RegNum);
	ReadData = read32();
    spi_ss.high();
    
	return ReadData;
}

uint16_t Spi::Read_PW_First()
{
    uint16_t ReadData = 0;
    
    spi_ss.low();
	write8(READ_PW_FIRST);
	ReadData = read16();
    spi_ss.high();
    
	return ReadData>>5;
}

uint16_t Spi::Read_PW_STOP1()
{
    uint16_t ReadData = 0;
    
    spi_ss.low();
	write8(READ_PW_STOP1);
	ReadData = read16();
    spi_ss.high();
    
	return ReadData>>5;
}

uint32_t Spi::data_average(uint32_t *dtatzz,uint8_t num) /*定义两个参数：数组首地址与数组大小*/ 
{ 
      uint8_t i,j;
      uint32_t temp; 
      uint32_t data_temp0=0;
      for(i=0;i<num-1;i++) 
          for(j=i+1;j<num;j++) /*注意循环的上下限*/ 
              if(dtatzz[i]>dtatzz[j]) 
              { 
                   temp=dtatzz[i]; 
                   dtatzz[i]=dtatzz[j]; 
                   dtatzz[j]=temp; 
              }
      for(i=2;i<num-2;i++)
          data_temp0=data_temp0+dtatzz[i];
      data_temp0>>=2;
      return data_temp0;
}

uint16_t Spi::get_status()
{
    return m_status;
}

float Spi::MS1030_Flow(void)
{
    uint32_t time_up_down_result = 0;
    uint16_t Result_status = 0;
    uint16_t Result_PW_First = 0;
    uint16_t Result_PW_STOP1 = 0;
    uint8_t i = 0;
    uint32_t Result_up_reg[8] = {0};
    uint32_t Result_up_sum = 0;
    uint32_t Result_down_reg[8] = {0};
    uint32_t Result_down_sum = 0;
    uint32_t time_up_down_diff[8] = {0};
    
    float flow_result = 0.0f;
    
    Write_Order(INITIAL);
    
    Write_Order(START_TOF_RESTART);
    
    while(Intn_flag == 0);
    Intn_flag = 0;           //glear flag
    
    //GPIOA->POD ^= GPIO_PIN_8;//blink green on board led
    
    Result_status = Read_STAT();
    //CV_LOG("status: 0x%04X\r\n", Result_status);
    //log_info("status: 0x%04X\r\n", Result_status);
    
    m_status = Result_status;
    
    //Result_PW_First = Read_PW_First();
    //CV_LOG("PW_First: 0x%04X\r\n", Result_PW_First);
    //log_info("PW_First: 0x%04X\r\n", Result_PW_First);
    
    //Result_PW_STOP1=Read_PW_STOP1();
    //CV_LOG("PW_STOP1: 0x%04X\r\n", Result_PW_STOP1);
    //log_info("PW_STOP1: 0x%04X\r\n", Result_PW_STOP1);
    
    for(i=0;i<8;i++)
    {
        Result_up_reg[i] = Read_Reg(READ_TOF_UP_STOP1 + i);
    }
    //Result_up_sum = Read_Reg(READ_TOF_UP_STOP1 + i);
    
    for(i=0;i<8;i++)
    {
        Result_down_reg[i] = Read_Reg(READ_TOF_DOWN_STOP1 + i);
    }
    //Result_down_sum = Read_Reg(READ_TOF_DOWN_STOP1 + i);
    
    for(i=0;i<8;i++)
    {
        if(Result_up_reg[i] >= Result_down_reg[i])
        {
            time_up_down_diff[i] = Result_up_reg[i] - Result_down_reg[i];
        }
        else
        {
            time_up_down_diff[i] = Result_down_reg[i] - Result_up_reg[i];
        }
    }
    
    time_up_down_result = data_average(time_up_down_diff, 8);
    
    //CV_LOG("time_up_down_result: 0x%08X\r\n", time_up_down_result);
    //log_info("time_up_down_result: 0x%08X - %3.5lfus\r\n", time_up_down_result, (float)time_up_down_result/65536.0f);
    
    flow_result = (float)time_up_down_result/65536.0f/2.0f;
    
    return  flow_result;
}

float Spi::MS1030_Temper(void)
{
    uint16_t Result_status = 0;
    uint32_t Result_temperature_reg[4] = {0};
    uint32_t i = 0;
    float pt1000_res = 0.0f;
    float pt1000_c = 0.0f;
    
    Write_Order(INITIAL);
    Write_Order(START_TEMP);
    
    while(Intn_flag == 0);
    Intn_flag = 0;           //glear flag
    
    Result_status = Read_STAT();
    //CV_LOG("status: 0x%04X\r\n", Result_status);
    //log_info("status: 0x%04X\r\n", Result_status);
    
    m_status = Result_status;
    
    for(i=0;i<4;i++)
    {
        Result_temperature_reg[i] = Read_Reg(READ_TEMP_PT1 + i);
        //CV_LOG("pt[%d]: 0x%08X\r\n", i+1, Result_temperature_reg[i]);
        //log_info("pt[%d]: 0x%08X\r\n", i+1, Result_temperature_reg[i]);
    }
    
    pt1000_res = 1000.0f*(float)(Result_temperature_reg[1])/(float)(Result_temperature_reg[0]);
    
    for(i=0;i<sizeof(RT_PT1000);i++)
    {
        if(pt1000_res > RT_PT1000[i] && pt1000_res < RT_PT1000[i+1])
        {
            pt1000_c = i/10.0f;
            break;
        }
    }
    
    return pt1000_c;
}

float Spi::MS1030_Time_check(void)
{
    uint16_t Result_status = 0;
    uint32_t cal_reg = 0;
    
    Write_Order(INITIAL);
    Write_Order(START_CAL_RESONATOR);
    
    while(Intn_flag == 0);
    Intn_flag = 0;           //glear flag
    
    Result_status = Read_STAT();
    
    //CV_LOG("status: 0x%04X\r\n", Result_status);
    //log_info("status: 0x%04X\r\n", Result_status);
    
    m_status = Result_status;
    
    cal_reg = Read_Reg(READ_CAL_REG);
        
    return (float)cal_reg/65536.0f/488.28125f;
}

uint8_t Spi::config()
{
    uint32_t REG0 = 0;
    uint32_t REG1 = 0;
    uint32_t REG2 = 0;
    uint32_t REG3 = 0;
    uint32_t REG4 = 0;
    uint8_t  SPI_check_temp = 0;
        
    REG0=0x118a4940;
    REG1=0xa0640000;
    REG2=0x00000000;
    REG3=0x00000000;
    REG4=0x46cc0500;    
    
    spi_rstn.high();
    dwt.delay_us(1);
    spi_rstn.low();
    dwt.delay_ms(10);
    spi_rstn.high();
    dwt.delay_us(100);
    
    Write_Order(POR);
    
    Write_Reg(WRITE_REG0, REG0);
	Write_Reg(WRITE_REG1, REG1);
	Write_Reg(WRITE_REG2, REG2);
	Write_Reg(WRITE_REG3, REG3);
	Write_Reg(WRITE_REG4, REG4);
    
    Write_Order(INITIAL);
    
    SPI_check_temp= Read_REG0_L();
    CV_LOG("REG0_L: 0x%02x\r\n", SPI_check_temp);
    log_info("REG0_L: 0x%02x\r\n", SPI_check_temp);
    
    return SPI_check_temp;
}


