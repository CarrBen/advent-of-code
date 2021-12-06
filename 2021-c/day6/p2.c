#include <stdio.h>
#include <stdlib.h>
#include <string.h>

long int sumFish(long int fish[9]) {
    long int s = 0;
    for (int i = 0; i < 9; i++) {
        s += fish[i];
    }
    return s;
}

char* countsString(long int fish[9]) {
    char *string = malloc(512 * sizeof(char));
    memset(string, 0, 64 * sizeof(char));
    
    for (int i =0; i<9; i++) {
        snprintf(string + strlen(string), 40, "%ld", fish[i]);
        strcat(string + strlen(string), ",");
    }
    
    return string;
}

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 1024;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int startCount = 0;

	fgets(buffer, maxLineLength, file);
    
    long int fishCounts[9];
    memset(&fishCounts, 0, 9 * sizeof(long int));
    
    char *token = strtok(buffer, ",");
    while(token) {
        int age = 0;
        sscanf(token, "%d", &age);
        printf("Loaded fish with age %d\n", age);
        fishCounts[age]++;
        startCount++;
        token = strtok(NULL, ",");
    }

	printf("Found %d entries in %s.\n", startCount, filename);
    printf("%s\n", countsString(fishCounts));

    int days = 256;
    long int nextCounts[9];
    memset(&nextCounts, 0, 9 * sizeof(long int));
    for (int d=0; d<days; d++) {
        for (int i=8; i>=0; i--) {
            if (i > 0) {
                nextCounts[i-1] = fishCounts[i];
            } else {
                nextCounts[6] += fishCounts[0];
                nextCounts[8] = fishCounts[0];
            }
        }
        memcpy(&fishCounts, &nextCounts, 9 * sizeof(long int));
        printf("Day %d, fish %ld. Counts: %s\n", d+1, sumFish(fishCounts), countsString(fishCounts));
    }
    
	fclose(file);

	return 0;
}
