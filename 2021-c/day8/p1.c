#include <stdio.h>
#include <stdlib.h>
#include <string.h>

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 128;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int lineCount = 0;
	
	int digitCount = 0;

	while(fgets(buffer, maxLineLength, file))
	{
		lineCount++;
		char *uniques = "";
		char *output = "";
		
		uniques = strtok(buffer, "|");
		output = strtok(NULL, "|");
		
		//printf("%s | %s\n", uniques, output);
		
		char *token = strtok(output, " ");
		while(token){
			int tokenLen = strlen(token);
			
			char lastChar = token[tokenLen-1];
			if(lastChar == ' ' || lastChar == '\n') {
				tokenLen--;
			}
			
			//printf("%s len %d\n", token, tokenLen);
			if ((tokenLen >=2 && tokenLen <= 4) || tokenLen == 7) {
				digitCount++;
			}
			token = strtok(NULL, " ");
		}
		
	}

	printf("Found %d lines in %s.\n", lineCount, filename);

	printf("Counted %d digits\n", digitCount);
	
	fclose(file);

	return 0;
}
