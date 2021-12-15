#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <limits.h>
#include <stdbool.h>

#define WIDTH (100)
#define HEIGHT (100)

bool inRange(int x, int y) {
	return x>=0 && x<WIDTH*5 && y>=0 && y<HEIGHT*5;
}

void printGridChar(char grid[HEIGHT*5][WIDTH*5]) {
	if(WIDTH > 50) {
		//return;
	}
	
	for(int y=0; y<HEIGHT*5; y++) {
		for(int x=0; x<WIDTH*5; x++) {
			printf("%d,", grid[y][x]);
		}
		printf("\n");
	}
}

void printGridInt(int grid[HEIGHT*5][WIDTH*5]) {
	if(WIDTH > 50) {
		//return;
	}
	
	for(int y=0; y<HEIGHT*5; y++) {
		for(int x=0; x<WIDTH*5; x++) {
			printf("%5d,", grid[y][x]);
		}
		printf("\n");
	}
}

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 128;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int lineCount = 0;
	
	char grid[HEIGHT*5][WIDTH*5] = { { 0 } };
	
	while(fgets(buffer, maxLineLength, file))
	{
		for(int i=0; i<WIDTH; i++) {
			grid[lineCount][i] = buffer[i] - '0';
		}
		lineCount++;
	}
	
	for(int y=0; y<HEIGHT; y++) {
		for(int x=0; x<WIDTH; x++) {
			for(int iy=0; iy<5; iy++) {
				for(int ix=0; ix<5; ix++) {
					if(ix==0 && iy==0) {
						continue;
					}
					int v = grid[y][x] + ix + iy;
					if(v > 9) {
						v-=9;
					}
					grid[y + iy*HEIGHT][x + ix*WIDTH] = v;
				}
			}
		}
	}

	printf("Found %d lines in %s.\n", lineCount, filename);
	//printGridChar(grid);

	int riskGrid[HEIGHT*5][WIDTH*5] = { { 0 } };
	
	for(int i=0; i<10; i++) {
		for(int y=0; y<HEIGHT*5; y++) {
			for(int x=0; x<WIDTH*5; x++) {
				int risk = 0;
				if(inRange(x-1, y)) {
					//printf("x-1 ");
					risk = grid[y][x] + riskGrid[y][x-1];
				}
				if(inRange(x, y-1)) {
					//printf("y-1 ");
					int r = grid[y][x] + riskGrid[y-1][x];
					if(r < risk || risk == 0) {
						risk = r;
					}
				}
				if(inRange(x+1, y) && riskGrid[y][x+1] > 0) {
					//printf("x+1 ");
					int r = grid[y][x] + riskGrid[y][x+1];
					if(r < risk || risk == 0) {
						risk = r;
					}
				}
				if(inRange(x, y+1) && riskGrid[y+1][x] > 0) {
					//printf("y+1 ");
					int r = grid[y][x] + riskGrid[y+1][x];
					if(r < risk || risk == 0) {
						risk = r;
					}
				}
				if(risk < riskGrid[y][x] || riskGrid[y][x] == 0) {
					//printf("%d,%d  %d < %d @ %d\n", x, y, risk, riskGrid[y][x], grid[y][x]);
					riskGrid[y][x] = risk;
				}
			}
		}
		printf("%d\n", riskGrid[HEIGHT*5-1][WIDTH*5-1]);
	}
	
	//printGridInt(riskGrid);
	

	fclose(file);

	return 0;
}
