#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>

struct command {
    bool on;
    int xmin;
    int xmax;
    int ymin;
    int ymax;
    int zmin;
    int zmax;
};

void parseRange(char *str, int *min, int *max) {
    strtok(str, "="); // Axis ID letter and equals
    char *numbers = strtok(NULL, "=");
    
    char *minStr = strtok(numbers, ".");
    char *maxStr = strtok(NULL, ".");
    
    sscanf(minStr, "%d", min);
    sscanf(maxStr, "%d", max);
}

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 64;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int lineCount = 0;
	
	struct command *commands = malloc(512 * sizeof(struct command));
	int commandCount = 0;

	while(fgets(buffer, maxLineLength, file))
	{
		lineCount++;
		
		char *state = strtok(buffer, " ");
		char *coords = strtok(NULL, " ");
		
		char *xcoords = strtok(coords, ",");
		char *ycoords = strtok(NULL, ",");
		char *zcoords = strtok(NULL, ",");
		
		struct command *cmd = &commands[commandCount];
		commandCount++;
		
		cmd->on = strcmp(state, "on") == 0 ? true : false;
		parseRange(xcoords, &cmd->xmin, &cmd->xmax);
		parseRange(ycoords, &cmd->ymin, &cmd->ymax);
		parseRange(zcoords, &cmd->zmin, &cmd->zmax);
		
		printf("State: %d X:%d..%d Y:%d..%d Z:%d..%d\n",
		    cmd->on, cmd->xmin, cmd->xmax, cmd->ymin, cmd->ymax, 
		    cmd->zmin, cmd->zmax);
	}

	printf("Found %d lines in %s.\n", lineCount, filename);

	fclose(file);
	
	int cubesOn = 0;
	
	for(int x=-50; x<=50; x++) {
	    for(int y=-50; y<=50; y++) {
	        for(int z=-50; z<=50; z++) {
	            for(int c=commandCount-1; c>=0; c--) {
	                struct command cmd = commands[c];
	                
	                if(x>=cmd.xmin && x<=cmd.xmax && 
	                    y>=cmd.ymin && y<=cmd.ymax &&
	                    z>=cmd.zmin && z<=cmd.zmax) {
	                    if(cmd.on) {
	                        cubesOn++;
	                        break;
	                    }else{
	                        break;
	                    }
                    }
	            }
            }
        }
	
	}

    printf("Ended with %d cubes on\n", cubesOn);

	return 0;
}
