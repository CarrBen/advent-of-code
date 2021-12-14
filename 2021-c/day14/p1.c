#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <limits.h>

struct node {
	struct node *next;
	char value;
};

struct insert {
	struct insert *next;
	struct node *left;
	char value;
};

struct rule {
	char left;
	char right;
	char insert;
};

void printChain(struct node *chain) {
	struct node *n = chain;
	while(n) {
		printf("%c", n->value);
		n = n->next;
	}
	printf("\n");
}

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 16;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int chainLength = 0;
	int ruleCount = 0;
	
	struct node *chain = malloc(sizeof(struct node));
	struct node *current = NULL;
	chain->next = NULL;

	while(fgets(buffer, maxLineLength, file))
	{
		if(buffer[0] == '\n' || buffer[0] == '\0') {
			break;
		}
		
		int i = 0;
		while(buffer[i] != '\0' && buffer[i] != '\n' && i < maxLineLength) {
			if(current == NULL) {
				current = chain;
			} else {
				current->next = malloc(sizeof(struct node));
				current = current->next;
			}
			
			current->value = buffer[i];
			chainLength++;
			
			i++;
		}
		
	}
	
	struct rule rules[32];
	
	while(fgets(buffer, maxLineLength, file))
	{
		if(buffer[0] == '\n' || buffer[0] == '\0') {
			continue;
		}
		
		struct rule *r = &rules[ruleCount];
		
		char *left = strtok(buffer, " -> ");
		char *right = strtok(NULL, " -> ");
		
		r->left = left[0];
		r->right = left[1];
		r->insert = right[0];
		
		ruleCount++;
	}

	printf("Found %d chars in %s.\n", chainLength, filename);
	//printChain(chain);
	printf("Found %d rules in %s.\n", ruleCount, filename);
	
	for(int step=0; step<10; step++) {
		struct insert *inserts = NULL;
		
		struct node *pointer = chain;
		struct insert *insertPointer = inserts;
		while(pointer) {
			//printf("Node\n");
			for(int r=0; r<ruleCount; r++) {
				if(pointer->next == NULL) {
					break;
				}
				
				if(pointer->value == rules[r].left && pointer->next->value == rules[r].right) {
					if(insertPointer == NULL) {
						insertPointer = malloc(sizeof(struct insert));
						inserts = insertPointer;
					}else{
						insertPointer->next = malloc(sizeof(struct insert));
						insertPointer = insertPointer->next;
					}
						// Override the pointer to the next insert with the next item in the list!!!
					insertPointer->left = pointer;
					insertPointer->value = rules[r].insert;
				}
			}
			pointer = pointer->next;
		}
		
		insertPointer = inserts;
		while(insertPointer) {
			//printf("Insert %d\n", insertPointer);
			pointer = chain;
			while(pointer) {
				if(pointer == insertPointer->left) {
					struct node *currentNext = pointer->next;
					pointer->next = malloc(sizeof(struct node));
					pointer->next->next = currentNext;
					pointer->next->value = insertPointer->value;
					break;
				}
				pointer = pointer->next;
			}
			insertPointer = insertPointer->next;
		}
		
		int length = 0;
		pointer = chain;
		while(pointer) {
			length++;
			pointer = pointer->next;
		}
		
		printf("Step %d chain length %d\n", step, length);
	}
	
	int counts[256] = { 0 };
	struct node *pointer = chain;
	while(pointer) {
		counts[pointer->value]++;
		pointer = pointer->next;
	}
	
	int leastCommon = INT_MAX;
	int mostCommon = 0;
	
	for(int i=0; i<256; i++) {
		if(counts[i] > 0 && counts[i] < leastCommon) {
			leastCommon = counts[i];
		}
		if(counts[i] > mostCommon) {
			mostCommon = counts[i];
		}
	}

	printf("%d %d %d\n", leastCommon, mostCommon, mostCommon - leastCommon);
	
	fclose(file);

	return 0;
}
