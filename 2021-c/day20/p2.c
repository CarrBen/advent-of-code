#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#define GRID (512)

void step(char **src, char **dest, char lookup[512], int offset){
    for(int x=offset; x<GRID-offset; x++) {
        for(int y=offset; y<GRID-offset; y++) {
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
	char **grid1 = malloc(GRID * sizeof(size_t));
	char **grid2 = malloc(GRID * sizeof(size_t));
	int y = (GRID-100)/2;
	int x = (GRID-100)/2;
	for(int i=0; i<GRID; i++) {
	    grid1[i] = malloc(GRID * sizeof(char));
	}
	for(int i=0; i<GRID; i++) {
	    grid2[i] = malloc(GRID * sizeof(char));
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
	
	for(int s=0; s<50; s++) {
	    step(grid1, grid2, lookup, s+1);
	    for(int i=0; i<GRID; i++) {
	        memcpy(grid1[i], grid2[i], GRID * sizeof(char));
	        memset(grid2[i], 0, GRID * sizeof(char));
	    }
	}
	
	int litCount = 0;
	for(int xi=0; xi<GRID; xi++) {
	    for(int yi=0; yi<GRID; yi++) {
	        if(grid1[yi][xi] == '#') {
	            litCount++;
	        }
	    }
	}
	
	printf("Counted %d lit cells\n", litCount);

	fclose(file);

	return 0;
}
