#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>

#define WIDTH (2048)
#define HEIGHT (2048)

int gridSum(char grid[HEIGHT][WIDTH]) {
	int total = 0;
	for(int y=0; y<HEIGHT; y++) {
		for(int x=0; x<WIDTH; x++) {
			total += grid[y][x];
		}
	}
	return total;
}

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 32;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int coordLineCount = 0;
	int foldLineCount = 0;
	
	char grid[HEIGHT][WIDTH] = { { 0 } };
	char folds[16][16] = { { 0 } };
	
	int maxX = 0;
	int maxY = 0;

	while(fgets(buffer, maxLineLength, file))
	{
		char *first = strtok(buffer, ",");
		char *second = strtok(NULL, ",");
		
		if(second == NULL) {
			break; // Time for the folds
		}
		
		int x, y;
		sscanf(first, "%d", &x);
		sscanf(second, "%d", &y);
		
		if(x > maxX) {
			maxX = x;
		}
		
		if(y > maxY) {
			maxY = y;
		}
		
		grid[y][x] = 1;
		
		coordLineCount++;
	}
	
	while(fgets(buffer, maxLineLength, file))
	{
		char *fold = strtok(buffer, " ");
		char *along = strtok(NULL, " ");
		char *details = strtok(NULL, " ");
		
		strcpy(folds[foldLineCount], details);
		foldLineCount++;
	}
	
	printf("There are %d dots\n", gridSum(grid));
	
	for(int i=0; i<1; i++) {
		printf("Fold along %s\n", folds[i]);
		
		char *axis = strtok(folds[i], "=");
		char *pos = strtok(NULL, "=");
		int p = 0;
		sscanf(pos, "%d", &p);
		
		for(int y=0; y<HEIGHT; y++) {
			if(axis[0] == 'y') {
				for(int x=0; x<WIDTH; x++) {
					if(grid[y][x]) {
						grid[2*p - y][x] = 1;
					}
					grid[y][x] = 0;
				}
			} else {
				for(int x=0; x<WIDTH; x++) {
					if(grid[y][x]) {
						grid[y][2*p - x] = 1;
					}
					grid[y][x] = 0;
				}
			}
		}
	}

	printf("Found %d coord lines in %s.\n", coordLineCount, filename);
	printf("Max X: %d Max Y: %d\n", maxX, maxY);
	printf("Found %d fold lines in %s.\n", foldLineCount, filename);
	printf("There are %d dots\n", gridSum(grid));
	
	fclose(file);

	return 0;
}
