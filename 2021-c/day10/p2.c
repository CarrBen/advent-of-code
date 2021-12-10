#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>


int cmpfunc (const void * aa, const void * bb) {
	const long int a = *(long int*)aa;
	const long int b = *(long int*)bb;
	if(a>b) {
		return 1;
	}else if(b>a) {
		return -1;
	}
	return 0;
}

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 1024;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int lineCount = 0;
	int validLineCount = 0;
	
	long int scores[128] = { 0 };
	
	while(fgets(buffer, maxLineLength, file))
	{
		lineCount++;
		
		long int completeScore = 0;
		
		char stack[maxLineLength];
		int stackPointer = 0;
		char c = buffer[0];
		int index = 1;
		bool invalid = false;
		while(c != '\0' && c != '\n' && !invalid) {
			char expected = '\0';
			switch(c) {
				case '(':
					stack[stackPointer] = c + 1;
					stackPointer++;
					break;
				case '[':
				case '{':
				case '<':
					stack[stackPointer] = c + 2;
					stackPointer++;
					break;
				case ')':
				case '}':
				case ']':
				case '>':
					expected = stack[stackPointer-1];
					stackPointer--;
					
					if (c != expected) {
						invalid = true;
					}
					
					break;
				default:
					printf("Unexpected char %c\n", c);
					return 1;
			}
			c = buffer[index];
			index++;
		}
		
		if(invalid) {
			continue;
		}
		
		while(stackPointer > 0) {
			char expected = stack[stackPointer-1];
			stackPointer--;
			
			switch(expected) {
				case ')':
					completeScore *= 5;
					completeScore += 1;
					break;
				case '}':
					completeScore *= 5;
					completeScore += 3;
					break;
				case ']':
					completeScore *= 5;
					completeScore += 2;
					break;
				case '>':
					completeScore *= 5;
					completeScore += 4;
					break;
			}
		}
		scores[validLineCount] = completeScore;
		validLineCount++;
		//printf("Auto complete score was %ld.\n", completeScore);
	}

	printf("Found %d lines of which %d were valid in %s.\n", lineCount, validLineCount, filename);

	qsort(&scores, validLineCount, sizeof(long int), cmpfunc);
	
	for(int i = 0; i < validLineCount; i++) {
		printf("%ld\n", scores[i]);
	}
	
	printf("\n");
	printf("%ld\n", scores[(validLineCount-1)/2]);

	fclose(file);

	return 0;
}
