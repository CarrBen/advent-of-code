#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 1024;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int lineCount = 0;
	
	int syntaxScore = 0;

	while(fgets(buffer, maxLineLength, file))
	{
		lineCount++;
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
						switch(c) {
							case ')':
								syntaxScore += 3;
								break;
							case ']':
								syntaxScore += 57;
								break;
							case '}':
								syntaxScore += 1197;
								break;
							case '>':
								syntaxScore += 25137;
								break;
						}
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
	}

	printf("Found %d lines in %s.\n", lineCount, filename);
	printf("Syntax error score was %d.\n", syntaxScore);

	fclose(file);

	return 0;
}
