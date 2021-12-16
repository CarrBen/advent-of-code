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
	long int result;
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

void operate(long int *result, long int value, int type, int packets) {
	switch(type){
		case 0:
			printf("%ld + %ld\n", *result, value);
			*result += value;
			break;
		case 1:
			if(packets == 0) {
				*result = value;
			}else{
				printf("%ld * %ld\n", *result, value);
				*result *= value;
			}
			break;
		case 2:
			if(packets == 0) {
				*result = value;
			}
			if(value < *result) {
				printf("Min %ld\n", value);
				*result = value;
			}
			break;
		case 3:
			if(value > *result) {
				*result = value;
				printf("Max %ld\n", *result);
			}
			break;
		case 5:
			if(packets == 0) {
				*result = value;
			}else{
				printf("%ld > %ld\n", *result, value);
				*result = *result > value ? 1 : 0;
			}
			break;
		case 6:
			if(packets == 0) {
				*result = value;
			}else{
				printf("%ld < %ld\n", *result, value);
				*result = *result < value ? 1 : 0;
			}
			break;
		case 7:
			if(packets == 0) {
				*result = value;
			}else{
				printf("%ld == %ld\n", *result, value);
				*result = *result == value ? 1 : 0;
			}
			break;
	}
}

struct parsed parsePacket(char *bits, int start) {
	int version = getBits(bits, start, 3);
	int type = getBits(bits, start + 3, 3);
	
	versionSum += version;
	int processed = 6;
	int packets = 1;
	long int result = 0;
	
	long int num = 0;
	int next = 0;
	int n = 0;
	
	int lenType = 0;
	int expectedPackets = 0;
	int expectedBits = 0;
	int subProcessedBits = 0;
	int subProcessedPackets = 0;
	struct parsed parseResult;
	
	if(type == 4) {
		do {
			next = getBits(bits, start + processed, 1);
			processed++;
			n = getBits(bits, start + processed, 4);
			processed += 4;
			num <<= 4;
			num |= n;
		} while(next > 0);
		result = num;
		struct parsed pr = { processed, packets, result };
		return pr;
	}

	lenType = getBits(bits, start + processed, 1);
	processed++;
	if(lenType == 0) {
		expectedBits = (int)getBits(bits, start + processed, 15);
		processed += 15;
		printf("Expecting %d bits\n", expectedBits);
		while(subProcessedBits < expectedBits) {
			parseResult = parsePacket(bits, start + processed);
			subProcessedBits += parseResult.bits;
			processed += parseResult.bits;
			operate(&result, parseResult.result, type, packets - 1);
			packets += parseResult.packets;
			printf("B Processed %d packets and %d bits to get %ld\n", parseResult.packets, parseResult.bits, parseResult.result);
		}
		
	}else{
		expectedPackets = (int)getBits(bits, start + processed, 11);
		processed += 11;
		printf("Expecting %d packets\n", expectedPackets);
		while(subProcessedPackets < expectedPackets) {
			parseResult = parsePacket(bits, start + processed);
			processed += parseResult.bits;			
			operate(&result, parseResult.result, type, subProcessedPackets);
			packets += parseResult.packets;
			subProcessedPackets++;
			printf("P Processed %d packets and %d bits to get %ld\n", parseResult.packets, parseResult.bits, parseResult.result);
		}
	}
	
	printf("Result %ld\n", result);
	
	struct parsed pr = { processed, packets, result };
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
	
	struct parsed p = parsePacket(bits, 0);
		
	printf("Found %d chars in %s.\n", charCount, filename);
	
	printf("Result %ld after %d bits and %d packets\n", p.result, p.bits, p.packets);
	
	free(bits);
	free(buffer);

	return 0;
}
