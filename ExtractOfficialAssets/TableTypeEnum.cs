﻿namespace ExtractOfficialAssets;

public enum TableTypeEnum
{
    NONE = -1, // 0xFFFFFFFF
    CHAR = 0,
    BOOL = 1,
    INT = 2,
    UINT = 3,
    FLOAT = 4,
    DOUBLE = 5,
    LONGLONG = 6,
    STRING = 7,
    VECTOR_CHAR = 100, // 0x00000064
    VECTOR_BOOL = 101, // 0x00000065
    VECTOR_INT = 102, // 0x00000066
    VECTOR_UINT = 103, // 0x00000067
    VECTOR_FLOAT = 104, // 0x00000068
    VECTOR_DOUBLE = 105, // 0x00000069
    VECTOR_LONGLONG = 106, // 0x0000006A
    VECTOR_STRING = 107, // 0x0000006B
    SEQUENCE_CHAR = 200, // 0x000000C8
    SEQUENCE_BOOL = 201, // 0x000000C9
    SEQUENCE_INT = 202, // 0x000000CA
    SEQUENCE_UINT = 203, // 0x000000CB
    SEQUENCE_FLOAT = 204, // 0x000000CC
    SEQUENCE_DOUBLE = 205, // 0x000000CD
    SEQUENCE_LONGLONG = 206, // 0x000000CE
    SEQUENCE_STRING = 207, // 0x000000CF
    VECTOR_SEQUENCE_CHAR = 300, // 0x0000012C
    VECTOR_SEQUENCE_BOOL = 301, // 0x0000012D
    VECTOR_SEQUENCE_INT = 302, // 0x0000012E
    VECTOR_SEQUENCE_UINT = 303, // 0x0000012F
    VECTOR_SEQUENCE_FLOAT = 304, // 0x00000130
    VECTOR_SEQUENCE_DOUBLE = 305, // 0x00000131
    VECTOR_SEQUENCE_LONGLONG = 306, // 0x00000132
    VECTOR_SEQUENCE_STRING = 307, // 0x00000133
    VECTOR_VECTOR_CHAR = 400, // 0x00000190
    VECTOR_VECTOR_BOOL = 401, // 0x00000191
    VECTOR_VECTOR_INT = 402, // 0x00000192
    VECTOR_VECTOR_UINT = 403, // 0x00000193
    VECTOR_VECTOR_FLOAT = 404, // 0x00000194
    VECTOR_VECTOR_DOUBLE = 405, // 0x00000195
    VECTOR_VECTOR_LONGLONG = 406, // 0x00000196
    VECTOR_VECTOR_STRING = 407, // 0x00000197
}