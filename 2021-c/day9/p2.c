#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>
#include <limits.h>

#define WIDTH (100)
#define HEIGHT (100)

struct pos {
	int x;
	int y;
};

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

bool inRange(int x, int y) {
	return x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT;
}

void push(int x, int y, int* stackPointer, struct pos *stack) {
	stack[*stackPointer].x = x;
	stack[*stackPointer].y = y;
	(*stackPointer)++;
}

int countBasin(int x, int y, char counted[]) {
	int squares = 0;
	
	struct pos* stack = malloc(1000 * sizeof(stack));
	int stackPointer = 0;
	
	stack[stackPointer].x = x;
	stack[stackPointer].y = y;
	stackPointer++;
	
	while(stackPointer > 0) {
		struct pos p = stack[stackPointer-1];
		stackPointer--;
		
		if (!counted[idx(p.x, p.y)]) {
			counted[idx(p.x, p.y)] = 1;
			squares++;
			
			if (inRange(p.x+1, p.y)) {
				push(p.x + 1, p.y, &stackPointer, stack);
			}
			if (inRange(p.x-1, p.y)) {
				push(p.x - 1, p.y, &stackPointer, stack);
			}
			if (inRange(p.x, p.y+1)) {
				push(p.x, p.y + 1, &stackPointer, stack);
			}
			if (inRange(p.x, p.y-1)) {
				push(p.x, p.y - 1, &stackPointer, stack);
			}
		}
	}
	
	return squares;
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
	char counted[WIDTH * HEIGHT] = { 0 };

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
	if(WIDTH<20) {
		printMap(map);
		printf("\n");
	}

	for (int i=0; i<HEIGHT; i++) {
		for (int j=0; j<WIDTH; j++) {
			int cell = map[idx(j, i)];
			if (cell == 9) {
				counted[idx(j, i)] = 1;
			}
		}
	}
	
	if(WIDTH<20) {
		printMap(counted);
	}
	
	int basinSizes[1000] = { 0 };
	int basinCount = 0;
	for (int i=0; i<HEIGHT; i++) {
		for (int j=0; j<WIDTH; j++) {
			if (!counted[idx(j, i)]) {
				int size = countBasin(j, i, counted);
				//printf("Basin Size: %d\n", size);
				basinSizes[basinCount] = size;
				basinCount++;
				if(WIDTH<20) {
					printMap(counted);
				}
			}
		}
	}
	
	int biggest = INT_MAX;
	// Top 3 basins
	for(int b=0; b<3; b++) {
		int num = 0;
		for(int i=0; i<basinCount; i++) {
			if(basinSizes[i] > num && basinSizes[i] < biggest) {
				num = basinSizes[i];
			}
		}
		printf("%d\n", num);
		biggest = num;
	}

	return 0;
}
