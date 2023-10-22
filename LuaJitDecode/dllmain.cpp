// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"

#include <fstream>
#include <iostream>

#include "main.h"
struct Assertion {
    const std::string message;
};
extern "C" __declspec(dllexport) const char* Decode(const char* data, int len,int* outLen);

//BOOL APIENTRY DllMain( HMODULE hModule,
//                       DWORD  ul_reason_for_call,
//                       LPVOID lpReserved
//)
//{
//    switch (ul_reason_for_call)
//    {
//    case DLL_PROCESS_ATTACH:
//    case DLL_THREAD_ATTACH:
//    case DLL_THREAD_DETACH:
//    case DLL_PROCESS_DETACH:
//        break;
//    }
//    return TRUE;
//}

void assert(const bool& assertion, const std::string& message, const std::string& filePath, const std::string& function, const std::string& source, const uint32_t& line) {
    if (!assertion) throw Assertion{ .message = "Error running " + function + "()\nSource: " + source + ":" + std::to_string(line) + "\n\nFile: " + filePath + "\n\n" + message };
}

std::string byte_to_string(const uint8_t& byte) {
    char string[] = "0x00";
    uint8_t digit;

    for (uint8_t i = 2; i--;) {
        digit = (byte >> i * 4) & 0xF;
        string[3 - i] = digit >= 0xA ? 'A' + digit - 0xA : '0' + digit;
    }

    return string;
}
void handle_eptr(std::exception_ptr eptr) // passing by value is ok
{
    try {
        if (eptr) {
            std::rethrow_exception(eptr);
        }
    }
    catch (const std::exception& e) {
        std::cout << "Caught exception \"" << e.what() << "\"\n";
    }
}
__declspec(dllexport) const char* Decode(const char* data, int len, int* outLen)
{
    std::exception_ptr eptr;
	try
	{
        //printf("%c%c%c%c\r\n", data[0], data[1], data[2], data[3]);
        Bytecode bytecode(data, len);
        //printf("bytecode\r\n");
        Ast ast(bytecode);
        //printf("ast\r\n");
        Lua lua(bytecode, ast);
        printf("lua\r\n");

        bytecode();
        printf("do bytecode\r\n");
        ast();
        printf("do ast\r\n");
        lua();
        printf("do lua\r\n");
        *outLen = lua.GetOutSize();

        //printf("do lua\r\n");
        auto str = lua.GetData();
        auto ret = new char[*outLen];
        memcpy(ret, str, *outLen);
        return ret;

	}
	catch (...)
	{
        eptr = std::current_exception();
	}
    handle_eptr(eptr);
    return nullptr;
}


int __cdecl main(void)
{
    std::ifstream t("E:\\Downloads\\New folder\\540738689.bytes", std::ios::out | std::ios::binary);
    t.seekg(0, std::ios::end);
    size_t size = t.tellg();
    char* data = new char[size];
    t.seekg(0);
    t.read(data, size);
    t.close();
    int outLen = 0;
    std::cout << Decode(data, size, &outLen) << std::endl;
    return 0;
}
