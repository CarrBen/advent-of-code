#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#define WIDTH (100)
#define HEIGHT (100)

void printMap(char map[]) {
	for (int i=0; i<HEIGHT; i++) {
		for (int j=0; j<WIDTH; j++) {
			printf("%d,", map[i * WIDTH + j]);
		}
		printf("\n");
	}
}

int idx(int x, int y) {
	return y * WIDTH + x;
}

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = WIDTH + 2;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int lineCount = 0;
	
	char map[WIDTH * HEIGHT] = { 0 };

	while(fgets(buffer, maxLineLength, file))
	{
		int charCount = 0;
		while(buffer[charCount] != '\n' && buffer[charCount] != '\0') {
			map[lineCount * WIDTH + charCount] = buffer[charCount] - '0';
			charCount++;
		}
		lineCount++;
	}

	fclose(file);
	printf("Found %d lines in %s.\n", lineCount, filename);
	//printMap(map);

	int riskSum = 0;
	for (int i=0; i<HEIGHT; i++) {
		for (int j=0; j<WIDTH; j++) {
			int cell = map[idx(j, i)];
			if (j<WIDTH-1 && map[idx(j+1, i)] <= cell) {
				// Right
				continue;
			} else if (j>0 && map[idx(j-1, i)] <= cell) {
				// Left
				continue;
			} else if (i>0 && map[idx(j, i-1)] <= cell) {
				// Up
				continue;
			} else if (i<HEIGHT-1 && map[idx(j, i+1)] <= cell) {
				// Down
				continue;
			}
			riskSum += cell+1;
		}
	}
	
	printf("Risk Sum for map is %d\n", riskSum);

	return 0;
}
