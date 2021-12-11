#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>

bool inRange(char x, char y) {
	return x>=0 && x<10 && y>=0 && y<10;
}

void printGrid(char grid[10][10]) {
	for(int y=0; y<10; y++) {
		for(int x=0; x<10; x++) {
			printf("%d", grid[y][x]);
		}
		printf("\n");
	}
}

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 16;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int lineCount = 0;
	
	char grid[10][10] = { { 0 } };
	char flashes[10][10] = { { 0 } };

	while(fgets(buffer, maxLineLength, file))
	{
		for(int i=0; i<10; i++) {
			grid[lineCount][i] = buffer[i] - '0';
		}
		lineCount++;
	}

	printf("Found %d lines in %s.\n", lineCount, filename);
	printGrid(grid);

	fclose(file);
	
	int flashCount = 0;

	for(int step=0; step<1000; step++) {
		int startingFlashCount = flashCount;
		for(int y=0; y<10; y++) {
			for(int x=0; x<10; x++) {
				grid[y][x]++;
			}
		}
		
		memset(&flashes, 0, 10 * 10 * sizeof(char));
		
		bool anyNines = false;
		do {
			printf("Step %d checking nines\n", step);
			anyNines = false;
			for(int y=0; y<10; y++) {
				for(int x=0; x<10; x++) {
					if(grid[y][x] > 9 && flashes[y][x] == 0) {
						anyNines = true;
						// Flash!
						flashCount++;
						flashes[y][x]++;
						
						for(int dx=-1; dx<=1; dx++) {
							for(int dy=-1; dy<=1; dy++) {
								if(inRange(x+dx, y+dy)) {
									grid[y+dy][x+dx]++;
								}
							}
						}
					}
				}
			}
		}while(anyNines);
		
		for(int y=0; y<10; y++) {
			for(int x=0; x<10; x++) {
				if(grid[y][x] > 9) {
					grid[y][x] = 0;
				}
			}
		}
		
		printGrid(flashes);
		
		if(flashCount - startingFlashCount == 10 * 10) {
			printf("100 flashes on step %d\n", step+1);
			break;
		}
	}
	
	printf("Counted %d flashes.\n", flashCount);

	return 0;
}
