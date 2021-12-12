#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>

struct cave {
	char name[6];
	bool small;
	struct cave** connections;
	int connectionCount;
};

void freeCave(struct cave cave) {
	free(cave.connections);
}

void printCave(struct cave* cave) {
	printf("Cave: %s\n", cave->name);
	printf("Small: %d\n", cave->small);
	printf("Connections:\n");
	for(int i=0; i<cave->connectionCount; i++) {
		printf("  %s\n", cave->connections[i]->name);
	}
	printf("\n");
}

struct path {
	struct cave** caves;
	int caveCount;
	bool cannotComplete;
};

void freePath(struct path path) {
	free(path.caves);
}

void printPath(struct path* path) {
	printf("Path:\n");
	for(int i=0; i<path->caveCount; i++) {
		printf("%s,", path->caves[i]->name);
	}
	printf("\n");
}

void addConnection(struct cave* caves, int *caveCount, char firstName[6], char secondName[6]) {
	if(secondName[strlen(secondName)-1] == '\n') {
		secondName[strlen(secondName)-1] = '\0';
	}

	struct cave *firstCave = NULL;
	struct cave *secondCave = NULL;
	
	for(int i=0; i<*caveCount; i++) {
		if(strcmp(caves[i].name, firstName) == 0) {
			firstCave = &(caves[i]);
		} else if(strcmp(caves[i].name, secondName) == 0) {
			secondCave = &(caves[i]);
		}
	}
	
	if (firstCave == NULL) {
		strcpy(caves[*caveCount].name, firstName);
		caves[*caveCount].small = (firstName[0] >= 'a' ? true : false);
		caves[*caveCount].connections = malloc(128 * sizeof(int));
		caves[*caveCount].connectionCount = 0;
		firstCave = &caves[*caveCount];
		(*caveCount)++;
	}
	if (secondCave == NULL) {
		strcpy(caves[*caveCount].name, secondName);
		caves[*caveCount].small = (secondName[0] >= 'a' ? true : false);
		caves[*caveCount].connections = malloc(128 * sizeof(int));
		caves[*caveCount].connectionCount = 0;
		secondCave = &caves[*caveCount];
		(*caveCount)++;
	}
	
	// Print before connecting to see if the correct cave was found in the array
	//printf("Adding caves %s %s\n", firstName, secondName);
	//printCave(firstCave);
	//printCave(secondCave);
	
	(*firstCave).connections[firstCave->connectionCount] = secondCave;
	(firstCave->connectionCount)++;
	(*secondCave).connections[secondCave->connectionCount] = firstCave;
	(secondCave->connectionCount)++;
	
	printf("Added caves %s %s\n", firstName, secondName);
	//printCave(firstCave);
	//printCave(secondCave);
}

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 32;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int lineCount = 0;
	
	struct cave *caves = malloc(32 * sizeof(struct cave));
	int caveCount = 0;

	while(fgets(buffer, maxLineLength, file))
	{
		char *firstCave = strtok(buffer, "-");
		char *secondCave = strtok(NULL, "-");
		
		addConnection(caves, &caveCount, firstCave, secondCave);
		
		lineCount++;
	}

	printf("Found %d lines in %s.\n", lineCount, filename);

	struct cave *startCave = NULL;
	struct cave *endCave = NULL;
	for(int i=0; i<caveCount; i++) {
		if(strcmp(caves[i].name, "start") == 0) {
			startCave = &caves[i];
		} else if(strcmp(caves[i].name, "end") == 0) {
			endCave = &caves[i];
		}
	}
	
	struct path *paths = malloc(16384 * sizeof(struct path));
	int pathCount = 0;
	int incompletePathCount = 1;
	
	struct path *p = &paths[pathCount];
	p->caves = malloc(128 * sizeof(struct cave*));
	p->caveCount = 0;
	p->caves[p->caveCount] = startCave;
	p->caveCount++;
	p->cannotComplete = false;
	pathCount++;
	
	while(incompletePathCount > 0) {
		printf("Incomplete paths: %d/%d\n", incompletePathCount, pathCount);
		int startingPathCount = pathCount;
		for(int pathIndex=0; pathIndex<startingPathCount; pathIndex++) {
			p = &paths[pathIndex];
			struct cave *lastCave = p->caves[p->caveCount-1];
			if(lastCave == endCave || p->cannotComplete) {
				continue;
			}
			
			printf("Path %d is not completed\n", pathIndex);
			
			struct cave* nextCaves[lastCave->connectionCount];
			int nextCaveCount = 0;
			for(int connIndex=0; connIndex<lastCave->connectionCount; connIndex++) {
				struct cave* candidateCave = lastCave->connections[connIndex];
				if(candidateCave->small) {
					bool duplicate = false;
					for(int pathCaveIndex=0; pathCaveIndex<p->caveCount; pathCaveIndex++) {
						if(candidateCave == p->caves[pathCaveIndex]) {
							duplicate = true;
							break;
						}
					}
					
					if(duplicate) {
						continue;
					}
				}
				nextCaves[nextCaveCount] = candidateCave;
				nextCaveCount++;
			}
			
			if(nextCaveCount == 0) {
				printf("Path %d has no next caves\n", pathIndex);
				printPath(p);
				p->cannotComplete = true;
				incompletePathCount--;
				continue;
			}
			
			for(int nextCaveIndex=1; nextCaveIndex<nextCaveCount; nextCaveIndex++) {
				struct path *np = &paths[pathCount];
				np->caves = malloc(32 * sizeof(struct cave*));
				memcpy(np->caves, p->caves, p->caveCount * sizeof(struct cave*));
				np->caveCount = p->caveCount;
				np->caves[np->caveCount] = nextCaves[nextCaveIndex];
				p->cannotComplete = false;
				np->caveCount++;
				pathCount++;
				if(nextCaves[nextCaveIndex] != endCave) {
					incompletePathCount++;
				} else {
					printf("Completed path\n");
					printPath(np);
				}
			}
			
			p->caves[p->caveCount] = nextCaves[0];
			p->caveCount++;
			if(nextCaves[0] == endCave) {
				incompletePathCount--;
				printf("Completed path\n");
				printPath(p);
			}
		}
	}
	
	fclose(file);
	int outputPathCount = 0;
	for(int i=0; i<pathCount; i++) {
		printPath(&paths[i]);
		if(!paths[i].cannotComplete) {
			outputPathCount++;
		}
		freePath(paths[i]);
	}
	printf("There were %d good paths\n", outputPathCount);
	free(paths);
	for(int i=0; i<caveCount; i++) {
		printCave(&caves[i]);
		freeCave(caves[i]);
	}
	free(caves);

	return 0;
}
