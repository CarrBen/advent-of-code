#include <stdio.h>
#include <stdlib.h>
#include <string.h>

int xmin = 0;
int xmax = 0;
int ymin = 0;
int ymax = 0;

int velocityValues;

int simulate(int ixv, int iyv) {
	int x = 0;
	int y = 0;
	int my = 0;
	int xv = ixv;
	int yv = iyv;
	while(y >= ymin && x <= xmax) {
		if(y > my) {
			my = y;
		}
		
		x += xv;
		y += yv;
		
		if(xv > 0) {
			xv -= 1;
		}else if(xv < 0) {
			xv += 1;
		}
		yv -= 1;
		
		//printf("Step %d,%d %d,%d\n", x, y, xv, yv);
		
		if(x >= xmin && x <= xmax && y >= ymin && y <= ymax) {
			//printf("In target area\n");
			if(y > my) {
				my = y;
			}
			velocityValues++;
			return my;
		}
	}
	
	return -1;
}

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 64;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");

	fgets(buffer, maxLineLength, file);
	
	strtok(buffer, " "); // Description
	strtok(NULL, ":"); // Description
	char* xtext = strtok(NULL, ", ");
	char* ytext = strtok(NULL, ", ");
	
	strtok(xtext, "=");
	char *str = strtok(NULL, "..");
	sscanf(str, "%d", &xmin);
	str = strtok(NULL, "..");
	sscanf(str, "%d", &xmax);
	
	strtok(ytext, "=");
	sscanf(strtok(NULL, ".."), "%d", &ymin);
	sscanf(strtok(NULL, ".."), "%d", &ymax);

	printf("Found Area X=%d - %d  Y=%d - %d\n", xmin, xmax, ymin, ymax);
	fclose(file);

	int maxy = 0;
	for(int x=-xmax; x<=xmax; x++) {
		for(int y=ymin; y<500; y++) {
			//printf("Sim %d,%d\n", x, y);
			int my =  simulate(x, y);
			if(my > maxy) {
				maxy = my;
			}
		}
	}
	
	printf("Max Y was %d\n", maxy);
	
	printf("Velocity Values %d\n", velocityValues);

	return 0;
}
