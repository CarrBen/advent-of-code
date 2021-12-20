#include <stdio.h>
#include <stdlib.h>
#include <string.h>

void step(char **src, char **dest, char lookup[512], int offset){
    for(int x=offset; x<128-offset; x++) {
        for(int y=offset; y<128-offset; y++) {
            int acc = 0;
            for(int j=-1; j<=1; j++) {
                for(int i=-1; i<=1; i++) {
                    acc += src[y+j][x+i] == '#' ? 1 : 0;
                    acc <<= 1;
                }
            }
            acc >>= 1; // Undo last shift
            //printf("Lookup %d,%d  %d\n", x, y, acc);
            dest[y][x] = lookup[acc];
        }
    }
}

void printGrid(char **grid, short w, short h) {
    for(int y=0; y<h; y++) {
        for(int x=0; x<w; x++) {
            printf("%c", grid[y][x]);
        }
        printf("\n");
    }
}

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 514;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int lineCount = 0;
	char lookup[512] = "";
	char **grid1 = malloc(128 * sizeof(size_t));
	char **grid2 = malloc(128 * sizeof(size_t));
	int y = 14;
	int x = 14;
	for(int i=0; i<128; i++) {
	    grid1[i] = malloc(128 * sizeof(char));
	}
	for(int i=0; i<128; i++) {
	    grid2[i] = malloc(128 * sizeof(char));
	}

	while(fgets(buffer, maxLineLength, file))
	{
		lineCount++;
		if(buffer[0] == '\n' || buffer[0] == '\0'){
		    continue;
		}
		if(lineCount == 1) {
		    for(int i=0; i<maxLineLength-2; i++) {
		        lookup[i] = buffer[i];
		    }
		    continue;
		}
		
		int lineLen = strlen(buffer);
		for(int i=0; i<lineLen; i++) {
		    grid1[y][x + i] = buffer[i];
		}
		y++;
	}
	
	printf("Read %d lines from %s\n", lineCount, filename);
	
	printGrid(grid1, 30, 30);
	step(grid1, grid2, lookup, 1);
	for(int i=0; i<128; i++) {
	    memset(grid1[i], 0, 128 * sizeof(char));
	}
	printf("\n");
	printGrid(grid2, 30, 30);
	step(grid2, grid1, lookup, 2);
	for(int i=0; i<128; i++) {
	    memset(grid2[i], 0, 128 * sizeof(char));
	}
	printf("\n");
	printGrid(grid1, 30, 30);
	
	int litCount = 0;
	for(int xi=0; xi<128; xi++) {
	    for(int yi=0; yi<128; yi++) {
	        if(grid1[yi][xi] == '#') {
	            litCount++;
	        }
	    }
	}
	
	printf("Counted %d lit cells\n", litCount);

	fclose(file);

	return 0;
}
