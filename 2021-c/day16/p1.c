#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>
#include <math.h>

int charCount = 0;
int versionSum = 0;

struct parsed {
	int bits;
	int packets;
};

char charToBits(char in) {
	if(in >=48 && in <= 57) {
		return in - '0';
	}
	return in - '7'; // 7 is 10 less than 'A' so 'A' - '7' = 10
}

void printChar(char in) {
	for(int i=7; i>=0; i--) {
		printf("%d", (in >> i) & 1);
	}
}

void printLong(long int in) {
	for(int i=63; i>=0; i--) {
		printf("%ld", (in >> i) & 1);
	}
}

void printBits(char* input, int len) {
	if (len>20) {
		return;
	}
	
	for(int i=0; i<len; i++) {
		printChar(input[i]);
	}
	printf("\n");
}

char* strToBits(char* input) {
	char* bits = malloc(1024 * sizeof(char));
	memset(bits, 0, 1024 * sizeof(char));
	
	int i = 0;
	bool leftHalf = true;
	char c = input[0];

	while(c != '\0' && c != '\n') {
		charCount++;
		char b = charToBits(c);
		
		if(leftHalf) {
			b = (char)(b << 4);;
		}
		leftHalf = !leftHalf;
		
		int bi = i/2;
		bits[bi] = b | bits[bi];
		
		i++;
		c = input[i];
		if(i == 2048) {
			printf("strToBits i was 2048\n");
			fflush(stdout);
			break;
		}
	}
	
	return bits;
}

long int getBits(char *bits, int offset, int len) {
	unsigned long int b = 0;
	if(len > 64) {
		printf("I can't deal with this! Offset %d Len %d\n", offset, len);
	}
	
	int charsFirst = offset / 8;
	int remainingOffset = offset - (charsFirst * 8);
	
	int extractedBits = 0;
	while(extractedBits < len) {
		// Extract to end from a starting point
		if(len - extractedBits >= 8 - remainingOffset) {
			unsigned char mask = 255 >> remainingOffset;
			b |= (bits[charsFirst] & mask);
			extractedBits += 8 - remainingOffset;
			remainingOffset = 0;
			charsFirst++;
			if(extractedBits < len) {
				b <<= (8 - remainingOffset);
			}
		}else{
			// Extract something that ends partway through (and maybe starts partway through too)
			int endOffset = 8 - (len - extractedBits) - remainingOffset;
			int bitsToGet = 8 - remainingOffset - endOffset;
			unsigned long int mask = (unsigned int)(pow(2, bitsToGet) - 1);
			mask <<= endOffset;
			b |= (bits[charsFirst] & mask);
			b >>= endOffset;
			extractedBits += bitsToGet;
		}
	}

	return b;
}

struct parsed parsePacket(char *bits, int start) {
	int version = getBits(bits, start, 3);
	int type = getBits(bits, start + 3, 3);
	
	versionSum += version;
	int processed = 6;
	int packets = 1;
	
	int num = 0;
	int next = 0;
	int n = 0;
	
	int lenType = 0;
	int expectedPackets = 0;
	int expectedBits = 0;
	int subProcessed = 0;
	struct parsed parseResult;
	
	switch(type) {
		case 4:
			do {
				next = getBits(bits, start + processed, 1);
				processed++;
				n = getBits(bits, start + processed, 4);
				processed += 4;
				num <<= 4;
				num |= n;
			} while(next > 0);
			printf("Num %d\n", num);
			break;
		default:
			lenType = getBits(bits, start + processed, 1);
			processed++;
			if(lenType == 0) {
				expectedBits = (int)getBits(bits, start + processed, 15);
				processed += 15;
				printf("Expecting %d bits\n", expectedBits);
				while(subProcessed < expectedBits) {
					parseResult = parsePacket(bits, start + processed);
					subProcessed += parseResult.bits;
					processed += parseResult.bits;
				}
				
			}else{
				expectedPackets = (int)getBits(bits, start + processed, 11);
				processed += 11;
				printf("Expecting %d packets\n", expectedPackets);
				while(packets < expectedPackets + 1) {
					parseResult = parsePacket(bits, start + processed);
					packets += parseResult.packets;
					processed += parseResult.bits;
				}
			}
			break;
	}
	
	struct parsed pr = { processed, packets };
	return pr;
}

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 2048;
	char* buffer = malloc(maxLineLength * sizeof(char));
	char* filename = argv[1];

	file = fopen(filename, "r");
	fgets(buffer, maxLineLength, file);
	fclose(file);
	
	printf("%s", buffer);
	char* bits = strToBits(buffer);
	printBits(bits, strlen(buffer)/2);
	
	parsePacket(bits, 0);
		
	printf("Found %d chars in %s.\n", charCount, filename);
	
	printf("Version\n");
	printLong(getBits(bits, 0, 3));
	printf("\nType\n");
	printLong(getBits(bits, 3, 3));
	printf("\n");
	
	printf("Version Sum %d\n", versionSum);
	
	free(bits);
	free(buffer);

	return 0;
}
