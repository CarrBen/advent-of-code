#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>
#include <math.h>

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

enum side {
    FRONT,
    TOP,
    LEFT,
    RIGHT,
    BOTTOM,
    BACK
};

void printCommand(struct command cmd) {
	printf("State: %d X:%d..%d Y:%d..%d Z:%d..%d\n",
	    cmd.on, cmd.xmin, cmd.xmax, cmd.ymin, cmd.ymax, 
	    cmd.zmin, cmd.zmax);
}

bool intersection(struct command a, struct command b);

bool validCommand(struct command a);

long long unsigned int commandVolume(struct command a);

struct command intersectionCommand(struct command a, struct command b);

struct command getSecondOutsideFirst(struct command first, struct command second, enum side s);

long long unsigned int sumVolume(struct command *cuboids, int cuboidCount);

long long unsigned int turnedOff = 0;

void insertCommand(struct command *cuboids, int *cuboidCount, int cuboidLimit, struct command cmd) {
    printf("Command %d volume: %llu Cubes on before: %llu Cubiods: %d\n", cmd.on, commandVolume(cmd), sumVolume(cuboids, *cuboidCount), *cuboidCount);
    printCommand(cmd);
    
    for(int cu=0; cu<*cuboidCount; cu++) {
        struct command other = cuboids[cu];
        
        if(!validCommand(cmd)) return;
        if(!validCommand(other)) continue;
        
        /*
        printf("Test intersect %d %d\n", cu, intersection(cmd, other));
        printf("Cmd\n");
        printCommand(cmd);
        printf("Other\n");
        printCommand(other);
        printf("\n");
        */
        
        if(intersection(cmd, other)) {
            if(cmd.on) {
                struct command front = getSecondOutsideFirst(other, cmd, FRONT);
                struct command back = getSecondOutsideFirst(other, cmd, BACK);
                struct command top = getSecondOutsideFirst(other, cmd, TOP);
                struct command left = getSecondOutsideFirst(other, cmd, LEFT);
                struct command right = getSecondOutsideFirst(other, cmd, RIGHT);
                struct command bottom = getSecondOutsideFirst(other, cmd, BOTTOM);
                
                printf("Intersect with %d count %d\n", cu, *cuboidCount);
                /*
                printf("Cmd\n");
                printCommand(cmd);
                printf("Other\n");
                printCommand(other);
                printf("\n");
                */  
                front.on = true;
                back.on = true;
                left.on = true;
                right.on = true;
                top.on = true;
                bottom.on = true;
                
                /*
                printf("front %d\n", validCommand(front));
                printCommand(front);
                printf("back %d\n", validCommand(back));
                printCommand(back);
                printf("left %d\n", validCommand(left));
                printCommand(left);
                printf("right %d\n", validCommand(right));
                printCommand(right);
                printf("top %d\n", validCommand(top));
                printCommand(top);
                printf("bottom %d\n", validCommand(bottom));
                printCommand(bottom);
                */
                
                if(validCommand(front)) {
                    cmd = front;
                }else{
                    cmd.xmin = 10;
                    cmd.xmax = 0; 
                }
                if(validCommand(back)) {
                    insertCommand(cuboids, cuboidCount, cuboidLimit, back);
                }
                if(validCommand(top)) {
                    insertCommand(cuboids, cuboidCount, cuboidLimit, top);
                }
                if(validCommand(left)) {
                    insertCommand(cuboids, cuboidCount, cuboidLimit, left);
                }
                if(validCommand(right)) {
                    insertCommand(cuboids, cuboidCount, cuboidLimit, right);
                }
                if(validCommand(bottom)) {
                    insertCommand(cuboids, cuboidCount, cuboidLimit, bottom);
                }
            }else{
                long long unsigned int turningOff = commandVolume(intersectionCommand(cmd, other));
                turnedOff += turningOff;
                struct command front = getSecondOutsideFirst(cmd, other, FRONT);
                struct command back = getSecondOutsideFirst(cmd, other, BACK);
                struct command top = getSecondOutsideFirst(cmd, other, TOP);
                struct command left = getSecondOutsideFirst(cmd, other, LEFT);
                struct command right = getSecondOutsideFirst(cmd, other, RIGHT);
                struct command bottom = getSecondOutsideFirst(cmd, other, BOTTOM);
                
                front.on = true;
                back.on = true;
                left.on = true;
                right.on = true;
                top.on = true;
                bottom.on = true;
                
                printf("Off Intersect with %d count %d\n", cu, *cuboidCount);
                /*
                printf("Was on %llu turning off %llu\n", commandVolume(other), turningOff);
                printf("Cmd\n");
                printCommand(cmd);
                printf("Other\n");
                printCommand(other);
                printf("\n");
                */
                
                bool otherOverriden = false;
                
                if(validCommand(front)) {
                    cuboids[cu] = front;
                    otherOverriden = true;
                    printf("Front %llu\n", commandVolume(front));
                    //printCommand(front);
                }
                if(validCommand(back)) {
                    printf("Back %llu\n", commandVolume(back));
                    //printCommand(back);
                    if(!otherOverriden) {
                        cuboids[cu] = back;
                        otherOverriden = true;
                    }else{
                        cuboids[*cuboidCount] = back;
                        (*cuboidCount)++;
                    }
                }
                if(validCommand(top)) {
                    printf("Top %llu\n", commandVolume(top));
                    //printCommand(top);
                    if(!otherOverriden) {
                        cuboids[cu] = top;
                        otherOverriden = true;
                    }else{
                        cuboids[*cuboidCount] = top;
                        (*cuboidCount)++;
                    }
                }
                if(validCommand(left)) {
                    printf("Left %llu\n", commandVolume(left));
                    //printCommand(left);
                    if(!otherOverriden) {
                        cuboids[cu] = left;
                        otherOverriden = true;
                    }else{
                        cuboids[*cuboidCount] = left;
                        (*cuboidCount)++;
                    }
                }
                if(validCommand(right)) {
                    printf("Right %llu\n", commandVolume(right));
                    //printCommand(right);
                    if(!otherOverriden) {
                        cuboids[cu] = right;
                        otherOverriden = true;
                    }else{
                        cuboids[*cuboidCount] = right;
                        (*cuboidCount)++;
                    }
                }
                if(validCommand(bottom)) {
                    printf("Bottom %llu\n", commandVolume(bottom));
                    //printCommand(bottom);
                    if(!otherOverriden) {
                        cuboids[cu] = bottom;
                        otherOverriden = true;
                    }else{
                        cuboids[*cuboidCount] = bottom;
                        (*cuboidCount)++;
                    }
                }
                if(!otherOverriden) {
                    cuboids[cu].xmin = 10;
                    cuboids[cu].xmax = 0;
                    otherOverriden = true;
                }
            }
        }
    }
    
    // If this is an off command, we don't need to store the "leftovers"
    if(!cmd.on) {
        return;
    }
    
    // If we still have anything left, add it as a new cuboid
    if(validCommand(cmd)) {
        printf("Added\n");
        cuboids[*cuboidCount] = cmd;
        (*cuboidCount)++;
    }else{
        printf("Did not add\n");
        printCommand(cmd);
    }
}

bool validCommand(struct command cmd) {
    if(cmd.xmax < cmd.xmin || cmd.ymax < cmd.ymin || cmd.zmax < cmd.zmin) return false;

    return (cmd.xmax - cmd.xmin + 1) > 0 &&
         (cmd.ymax - cmd.ymin + 1) > 0 &&
         (cmd.zmax - cmd.zmin + 1) > 0;
}

bool inRange(int value, int min, int max) {
    return (value >= min && value <= max);
}

bool intersects(struct command a, struct command b) {
    bool fullyEnclosed = b.xmin <= a.xmin && b.xmax >= a.xmax;
    if(!inRange(b.xmin, a.xmin, a.xmax) && !inRange(b.xmax, a.xmin, a.xmax) && !fullyEnclosed) return false;
    fullyEnclosed = b.ymin <= a.ymin && b.ymax >= a.ymax;
    if(!inRange(b.ymin, a.ymin, a.ymax) && !inRange(b.ymax, a.ymin, a.ymax) && !fullyEnclosed) return false;
    fullyEnclosed = b.zmin <= a.zmin && b.zmax >= a.zmax;
    if(!inRange(b.zmin, a.zmin, a.zmax) && !inRange(b.zmax, a.zmin, a.zmax) && !fullyEnclosed) return false;
    return true;
}

bool intersection(struct command a, struct command b) {
    return intersects(a, b) || intersects(b, a);
}

struct command intersectionCommand(struct command a, struct command b) {
    struct command cmd;
    cmd.xmax = fmin(a.xmax, b.xmax);
    cmd.xmin = fmax(a.xmin, b.xmin);
    cmd.ymax = fmin(a.ymax, b.ymax);
    cmd.ymin = fmax(a.ymin, b.ymin);
    cmd.zmax = fmin(a.zmax, b.zmax);
    cmd.zmin = fmax(a.zmin, b.zmin);
    
    return cmd;
}

long long unsigned int commandVolume(struct command a) {
    long long unsigned int xrange = a.xmax - a.xmin + 1;
    long long unsigned int yrange = a.ymax - a.ymin + 1;
    long long unsigned int zrange = a.zmax - a.zmin + 1;
    
    return xrange * yrange * zrange;
}

struct command getSecondOutsideFirst(struct command first, struct command second, enum side s) {
    struct command inter = intersectionCommand(first, second);
    struct command cmd;
    
    switch(s) {
        case FRONT: // X+
            cmd.xmin = inter.xmax + 1;
            cmd.xmax = second.xmax > first.xmax ? second.xmax : first.xmin;
            cmd.ymin = second.ymin;
            cmd.ymax = second.ymax;
            cmd.zmin = second.zmin;
            cmd.zmax = second.zmax;
            break;
        case BACK: // X-
            cmd.xmin = second.xmin < first.xmin ? second.xmin : first.xmax;
            cmd.xmax = inter.xmin - 1;
            cmd.ymin = second.ymin;
            cmd.ymax = second.ymax;
            cmd.zmin = second.zmin;
            cmd.zmax = second.zmax;
            break;
        case TOP: // Z+
            cmd.xmin = inter.xmin;
            cmd.xmax = inter.xmax;
            cmd.ymin = inter.ymin;
            cmd.ymax = inter.ymax;
            cmd.zmin = inter.zmax + 1;
            cmd.zmax = second.zmax > first.zmax ? second.zmax : first.zmin;
            break;
        case LEFT: // Y+
            cmd.xmin = inter.xmin;
            cmd.xmax = inter.xmax;
            cmd.ymin = inter.ymax + 1;
            cmd.ymax = second.ymax > first.ymax ? second.ymax : first.ymin;
            cmd.zmin = second.zmin;
            cmd.zmax = second.zmax;
            break;
        case RIGHT: // Y-
            cmd.xmin = inter.xmin;
            cmd.xmax = inter.xmax;
            cmd.ymin = second.ymin < first.ymin ? second.ymin : first.ymax;
            cmd.ymax = inter.ymin - 1;
            cmd.zmin = second.zmin;
            cmd.zmax = second.zmax;
            break;
        case BOTTOM: // Z-
            cmd.xmin = inter.xmin;
            cmd.xmax = inter.xmax;
            cmd.ymin = inter.ymin;
            cmd.ymax = inter.ymax;
            cmd.zmin = second.zmin < first.zmin ? second.zmin : first.zmax;
            cmd.zmax = inter.zmin - 1;
            break;
    }
    
    return cmd;
};

long long unsigned int sumVolume(struct command *cuboids, int cuboidCount) {
    long long unsigned int cubesOn = 0;
    for(int cu=0; cu<cuboidCount; cu++) {
        struct command cmd = cuboids[cu];
        //printCommand(cmd);
        if(!cmd.on) {
            printf("Found bad command (off) in final list \n");
            continue;
        };
        if(!validCommand(cmd)) {
            printf("Found bad command (invalid) in final list \n");
            continue;
        };
        cubesOn += commandVolume(cmd);
    }
    
    return cubesOn;
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
	
	int cuboidLimit = 8192;
	struct command *cuboids = malloc(cuboidLimit * sizeof(struct command));
	memset(cuboids, 0, cuboidLimit * sizeof(struct command));
	int cuboidCount = 0;
	
	for(int c=0; c<commandCount; c++) {
        printf("Processing command %d/%d\n", c, commandCount-1);
	    struct command cmd = commands[c];
	    
        if(cuboidCount > cuboidLimit - 16) {
            printf("%d is approaching cuboid limit of %d on command %d\n",
                cuboidCount, cuboidLimit, c);
        }
	    
	    insertCommand(cuboids, &cuboidCount, cuboidLimit, cmd);
	}

    long long unsigned int cubesOn = sumVolume(cuboids, cuboidCount);
    
    printf("Ended with %d cuboids\n", cuboidCount);
    printf("Ended with %llu cubes on\n", cubesOn);
    printf("%llu cubes were turned off at some point\n", turnedOff);

	return 0;
}
