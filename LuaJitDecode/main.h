#pragma once
#include "pch.h"
#include <bit>
#include <cmath>
#include <cstdint>
#include <string>
#include <vector>
#include <windows.h>
#include <fileapi.h>
#include <shlwapi.h>

#define DEBUG_INFO __FUNCTION__, __FILE__, __LINE__

constexpr char PROGRAM_NAME[] = "LuaJIT Decompiler v2";
constexpr uint64_t DOUBLE_SIGN = 0x8000000000000000;
constexpr uint64_t DOUBLE_EXPONENT = 0x7FF0000000000000;
constexpr uint64_t DOUBLE_FRACTION = 0x000FFFFFFFFFFFFF;
constexpr uint64_t DOUBLE_SPECIAL = DOUBLE_EXPONENT;
constexpr uint64_t DOUBLE_NEGATIVE_ZERO = DOUBLE_SIGN;
std::string byte_to_string(const uint8_t& byte);

void assert(const bool& assertion, const std::string& message, const std::string& filePath, const std::string& function, const std::string& source, const uint32_t& line);

class Bytecode;
class Ast;
class Lua;
#include "bytecode\bytecode.h"
#include "ast\ast.h"
#include "lua\lua.h"
