#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#define GRID_WIDTH (1024)
#define GRID_HEIGHT (1024)

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 32;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int lineCount = 0;
	int ignoredCount = 0;
	unsigned short *grid = malloc(GRID_WIDTH * GRID_HEIGHT * sizeof(short));
	memset(grid, 0, GRID_WIDTH * GRID_HEIGHT * sizeof(short));

	while(fgets(buffer, maxLineLength, file))
	{
		lineCount++;
		
		char *start = strtok(buffer, " -> ");
		char *end = strtok(NULL, " -> ");
		
		unsigned short x_start = 0;
		unsigned short y_start = 0;
		unsigned short x_end = 0;
		unsigned short y_end = 0;
		
		sscanf(strtok(start, ","), "%hu", &x_start);
		sscanf(strtok(NULL, ","), "%hu", &y_start);
		
		sscanf(strtok(end, ","), "%hu", &x_end);
		sscanf(strtok(NULL, ","), "%hu", &y_end);
		
		if(x_start != x_end && y_start != y_end) {
			printf("Line %d,%d -> %d,%d is diagonal!\n", x_start, y_start, x_end, y_end);
			ignoredCount++;
			continue;
		}
		
		if(x_start > x_end) {
			int swap = x_start;
			x_start = x_end;
			x_end = swap;
		}
		
		if(y_start > y_end) {
			int swap = y_start;
			y_start = y_end;
			y_end = swap;
		}
		
		//printf("Marking line %d,%d -> %d,%d\n", x_start, y_start, x_end, y_end);
		for(int x=x_start; x<=x_end; x++) {
			for(int y=y_start; y<=y_end; y++) {
				//printf("Marked %d,%d\n", x, y);
				int gridIndex = y * GRID_WIDTH + x;
				grid[gridIndex]++;
			}
		}
	}

	printf("Found %d lines in %s. %d were ignored for being diagonal.\n", lineCount, filename, ignoredCount);

	fclose(file);

	int dangerAreaCount = 0;
	for(int x=0; x<GRID_WIDTH; x++) {
		for(int y=0; y<GRID_HEIGHT; y++) {
			int gridIndex = y * GRID_WIDTH + x;
			if(grid[gridIndex] > 1) {
				dangerAreaCount++;
			}
		}
	}
	
	printf("Found %d danger areas", dangerAreaCount);
	
	free(grid);

	return 0;
}
