#include "pch.h"

#include "..\main.h"

Bytecode::Bytecode(const char* fileData, int len) 
{
	bytesUnread = len;
	fileSize = len;
	this->fileData = fileData;
}

Bytecode::~Bytecode() {
	close_file();

	for (uint64_t i = prototypes.size(); i--;) {
		delete prototypes[i];
	}
}

void Bytecode::operator()() {
	//print_progress_bar();
	open_file();
	//printf("Bytecode::read_file open_file()\r\n");
	read_header();
	//printf("Bytecode::read_file read_header()\r\n");
	prototypesTotalSize = bytesUnread - 1;
	read_prototypes();
	//printf("Bytecode::read_file read_prototypes()\r\n");
	close_file();
	//printf("Bytecode::read_file close_file()\r\n");
	fileBuffer.clear();
	fileBuffer.shrink_to_fit();
	//erase_progress_bar();
}

void Bytecode::read_header() {
	read_file(5);
	//printf("Bytecode::read_header() 0\r\n");
	assert(fileBuffer[0] == BC_HEADER[0] && fileBuffer[1] == BC_HEADER[1] && fileBuffer[2] == BC_HEADER[2],
		"Invalid header:\nExpected bytes " + byte_to_string(BC_HEADER[0]) + " " + byte_to_string(BC_HEADER[1]) + " " + byte_to_string(BC_HEADER[2])
		+ ", got " + byte_to_string(fileBuffer[0]) + " " + byte_to_string(fileBuffer[1]) + " " + byte_to_string(fileBuffer[2])
		+ "\n\nFile does not contain valid LuaJIT bytecode", "Decode", DEBUG_INFO);

	//printf("Bytecode::read_header() 1\r\n");
	header.version = fileBuffer[3];
	assert(header.version == BC_VERSION_1 || header.version == BC_VERSION_2, "Invalid bytecode version (" + byte_to_string(fileBuffer[3]) + ")", "Decode", DEBUG_INFO);
	header.flags = fileBuffer[4];
	assert(!(header.flags & ~(BC_F_BE | BC_F_STRIP | BC_F_FFI | (header.version == BC_VERSION_2 ? BC_F_FR2 : 0))), "Invalid flags (" + byte_to_string(header.flags) + ")", "Decode", DEBUG_INFO);
	assert(!(header.flags & BC_F_BE), "Big endian support not implemented", "Decode", DEBUG_INFO); //TODO

	//printf("Bytecode::read_header() 2\r\n");
	if (header.flags & BC_F_STRIP) return;
	read_file(read_uleb128());
	header.chunkname.resize(fileBuffer.size());
	header.chunkname.replace(header.chunkname.begin(), header.chunkname.end(), fileBuffer.begin(), fileBuffer.end());
}

void Bytecode::read_prototypes() {
	std::vector<Prototype*> unlinkedPrototypes;

	//printf("Bytecode::read_prototypes() 0\r\n");
	while (buffer_next_block()) {
		assert(fileBuffer.size() >= MIN_PROTO_SIZE, "Prototype is too short", "Decode", DEBUG_INFO);
		prototypes.emplace_back(new Prototype(*this));
		prototypes.back()->prototypeSize = fileBuffer.size();
		(*prototypes.back())(unlinkedPrototypes);
		//print_progress_bar(prototypesTotalSize - bytesUnread - 1, prototypesTotalSize);
	}

	//printf("Bytecode::read_prototypes() 1\r\n");
	assert(unlinkedPrototypes.size() == 1, "Failed to link main prototype", "Decode", DEBUG_INFO);
	main = unlinkedPrototypes.back();
	prototypes.shrink_to_fit();
}

void Bytecode::open_file() {
	/*file = CreateFileA(filePath.c_str(), GENERIC_READ, NULL, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL | FILE_FLAG_SEQUENTIAL_SCAN, NULL);
	assert(file != INVALID_HANDLE_VALUE, "Unable to open file", filePath, DEBUG_INFO);
	DWORD fileSizeHigh = 0;
	fileSize = GetFileSize(file, &fileSizeHigh);
	fileSize |= (uint64_t)fileSizeHigh << 32;
	assert(fileSize >= MIN_FILE_SIZE, "File is too small or empty", filePath, DEBUG_INFO);
	bytesUnread = fileSize;*/
}

void Bytecode::close_file() {
	/*if (file == INVALID_HANDLE_VALUE) return;
	CloseHandle(file);
	file = INVALID_HANDLE_VALUE;*/

}

void Bytecode::read_file(const uint32_t& byteCount) {

	//printf("Bytecode::read_file %d\r\n", byteCount);
	assert(bytesUnread >= byteCount, "Read would exceed end of file", "Decode", DEBUG_INFO);
	fileBuffer.resize(byteCount);
	DWORD bytesRead = 0;
	//assert(ReadFile(file, fileBuffer.data(), byteCount, &bytesRead, NULL) && !(byteCount - bytesRead), "Failed to read file", filePath, DEBUG_INFO);

	//printf("Bytecode::read_file fileBuffer.resize:  %d\r\n", fileBuffer.size());
	for(auto i= 0;i< byteCount; i++)
	{
		//printf("Bytecode::read_file pos %d : %d\r\n", i , fileData[filePosition + i]);
		fileBuffer[i]=(fileData[filePosition + i]);
	}
	//printf("Bytecode::read_file end read\r\n");
	filePosition += byteCount;
	bytesUnread -= byteCount;
}

uint32_t Bytecode::read_uleb128() {
	read_file(1);
	uint32_t uleb128 = fileBuffer[0];

	if (uleb128 >= 0x80) {
		uleb128 &= 0x7F;
		uint8_t bitShift = 0;

		do {
			bitShift += 7;
			read_file(1);
			uleb128 |= (fileBuffer[0] & 0x7F) << bitShift;
		} while (fileBuffer[0] >= 0x80);
	}

	return uleb128;
}

bool Bytecode::buffer_next_block() {
	const uint32_t byteCount = read_uleb128();

	if (!byteCount) {
		assert(!bytesUnread, "Read unexpectedly reached end of file", "Decode", DEBUG_INFO);
		return false;
	}

	read_file(byteCount);
	return true;
}
