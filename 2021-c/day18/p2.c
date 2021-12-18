#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>

#define DEBUG (false)

struct node {
	struct node *leftNode;
	int leftNumber;
	struct node *rightNode;
	int rightNumber;
};

void printNumber(struct node *root, bool newline) {
	printf("[");
	if(root->leftNode != NULL) {
		printNumber(root->leftNode, false);
	}else{
		printf("%d", root->leftNumber);
	}
	printf(",");
	if(root->rightNode != NULL) {
		printNumber(root->rightNode, false);
	}else{
		printf("%d", root->rightNumber);
	}
	printf("]");
	if(newline) {
		printf("\n");
	}
}

bool hasVisited(struct node *node, struct node **visited) {
	for(int i=0; i<128; i++) {
		if(visited[i] == node) {
			return true;
		}
	}
	
	return false;
}

// -1 for no action taken
// 1 for explode
// 2 for split
int reduce(struct node *root) {
	struct node **stack = malloc(128 * sizeof(size_t));
	struct node **visited = malloc(128 * sizeof(size_t));
	memset(visited, 0, 128 * sizeof(size_t));
	int visitedCount = 0;
	struct node *current = root;
	int stackPointer = 0;
	
	if(DEBUG) {
		printf("Reducing\n");
		printNumber(root, true);
	}
	
	while(stackPointer >= 0) {
		if(current->leftNode != NULL && !hasVisited(current->leftNode, visited)) {
			stack[stackPointer] = current;
			stackPointer++;
			current = current->leftNode;
			visited[visitedCount] = current;
			visitedCount++;
			continue;
		}
		
		if(current->rightNode != NULL && !hasVisited(current->rightNode, visited)) {
			stack[stackPointer] = current;
			stackPointer++;
			current = current->rightNode;
			visited[visitedCount] = current;
			visitedCount++;
			continue;
		}
		
		//printf("StackPointer %d ", stackPointer);
		//printNumber(current, true);
		if(current->leftNode == NULL && current->rightNode == NULL && stackPointer > 3) {
			//Explode
			if(DEBUG) {
				printf("Explode ");
				printNumber(current, true);
			}
			struct node *exploder = current;
			struct node *previous = current;
			struct node **explodeStack = malloc(128 * sizeof(size_t));
			memcpy(explodeStack, stack, 128 * sizeof(size_t));
			int explodeStackPointer = stackPointer;
			
			// Left number
			while(explodeStackPointer >= 0) {
				//printf("Looping ");
				//printNumber(current, true);
				if(current->rightNode == previous) {
					// We've just come up out of the right node
					if(current->leftNode != NULL) {
						//printf("Down left\n");
						explodeStack[explodeStackPointer] = current;
						explodeStackPointer++;
						previous = current;
						current = current->leftNode;
						continue;
					}else{
						current->leftNumber += exploder->leftNumber;
						break;
					}
				}else if(current->leftNode != previous && previous != current) {
					// We didn't come up out of the left node, so we went down
					//printf("Down to ");
					//printNumber(current, true);
					if(current->rightNode == NULL) {
						//printf("Add\n");
						current->rightNumber += exploder->leftNumber;
						break;
					}else{
						//printf("Down right\n");
						explodeStack[explodeStackPointer] = current;
						explodeStackPointer++;
						previous = current;
						current = current->rightNode;
						continue;
					}
				}
				
				explodeStackPointer--;
				if(explodeStackPointer < 0) {
					break;
				}
				previous = current;
				current = explodeStack[explodeStackPointer];
			}
			
			// Right Number
			memcpy(explodeStack, stack, 128 * sizeof(size_t));
			explodeStackPointer = stackPointer;
			
			explodeStackPointer--;
			previous = exploder;
			current = explodeStack[explodeStackPointer];
			
			if(current->leftNode != previous) {
				while(current->rightNode == previous) {
					explodeStackPointer--;
					if(explodeStackPointer < 0) {
						break;
					}
					previous = current;
					current = explodeStack[explodeStackPointer];
				}
				
				if(current->rightNode == previous) {
					explodeStackPointer = -1;
				}
			}else{
				if(current->rightNode == NULL) {
					current->rightNumber += exploder->rightNumber;
					explodeStackPointer = -1;
				}
			}
				
			if(current->rightNode != NULL && explodeStackPointer >= 0) {
				explodeStack[explodeStackPointer] = current;
				explodeStackPointer++;
				previous = current;
				current = current->rightNode;
			}else{
				if(current->rightNode == NULL && explodeStackPointer >= 0) {
					current->rightNumber += exploder->rightNumber;
				}
				explodeStackPointer = -1;
			}
			
			while(explodeStackPointer >= 0) {
				if(current->leftNode == NULL) {
					current->leftNumber += exploder->rightNumber;
					break;
				}
				
				explodeStack[explodeStackPointer] = current;
				explodeStackPointer++;
				previous = current;
				current = current->leftNode;
			}
			
			// Replace pair with zero
			bool replaced = false;
			if(stack[stackPointer-1]->leftNode == exploder) {
				stack[stackPointer-1]->leftNode = NULL;
				stack[stackPointer-1]->leftNumber = 0;
				replaced = true;
			}
			if(stack[stackPointer-1]->rightNode == exploder) {
				stack[stackPointer-1]->rightNode = NULL;
				stack[stackPointer-1]->rightNumber = 0;
				replaced = true;
			}
			if(!replaced) {
				printf("Exploded pair was not replaced\n");
			}
			
			free(explodeStack);
			free(stack);
			return 1;
		}
		
		stackPointer--;
		if(stackPointer < 0) {
			break;
		}
		current = stack[stackPointer];
	}
	
	memset(stack, 0, 128 * sizeof(size_t));
	memset(visited, 0, 128 * sizeof(size_t));
	visitedCount = 0;
	current = root;
	stackPointer = 0;
	
	while(stackPointer >= 0) {
		if(current->leftNode != NULL && !hasVisited(current->leftNode, visited)) {
			stack[stackPointer] = current;
			stackPointer++;
			current = current->leftNode;
			visited[visitedCount] = current;
			visitedCount++;
			continue;
		}
		
		if(current->leftNode == NULL && current->leftNumber >= 10) {
			// Not even keeping track of this to free it later...
			if(DEBUG) {
				printf("Split Left ");
				printNumber(current, true);
			}
			struct node *newNode = malloc(sizeof(struct node));
			current->leftNode = newNode;
			newNode->leftNumber = current->leftNumber/2;
			newNode->rightNumber = current->leftNumber/2;
			if((current->leftNumber/2) * 2 < current->leftNumber) {
				newNode->rightNumber++;
			}
			current->leftNumber = 0;
			free(stack);
			return 2;
		}
		
		if(current->rightNode != NULL && !hasVisited(current->rightNode, visited)) {
			stack[stackPointer] = current;
			stackPointer++;
			current = current->rightNode;
			visited[visitedCount] = current;
			visitedCount++;
			continue;
		}
			
		if(current->rightNode == NULL && current->rightNumber >= 10) {
			// Not even keeping track of this to free it later...
			if(DEBUG) {
				printf("Split Right ");
				printNumber(current, true);
			}
			struct node *newNode = malloc(sizeof(struct node));
			current->rightNode = newNode;
			newNode->leftNumber = current->rightNumber/2;
			newNode->rightNumber = current->rightNumber/2;
			if((current->rightNumber/2) * 2 < current->rightNumber) {
				newNode->rightNumber++;
			}
			current->rightNumber = 0;
			free(stack);
			return 2;
		}
		
		stackPointer--;
		if(stackPointer < 0) {
			break;
		}
		current = stack[stackPointer];
	}
	
	free(stack);
	return -1;
}

long int magnitude(struct node *p) {
	long int sum = 0;
	
	if(p->leftNode == NULL) {
		sum += 3 * p->leftNumber;
	}else{
		sum += 3 * magnitude(p->leftNode);
	}
	
	if(p->rightNode == NULL) {
		sum += 2 * p->rightNumber;
	}else{
		sum += 2 * magnitude(p->rightNode);
	}
	
	return sum;
}

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 128;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int lineCount = 0;
	
	struct node **input = malloc(128 * sizeof(size_t));
	
	struct node **stack = malloc(128 * sizeof(size_t));
	struct node *current = NULL;
	int stackPointer = 0;

	while(fgets(buffer, maxLineLength, file))
	{
		input[lineCount] = malloc(128 * sizeof(struct node));
		memset(input[lineCount], 0, 128 * sizeof(struct node));
		
		int i = 0;
		int nodeCount = 0;
		char c = buffer[i];
		
		struct node *lastNode = NULL;
		int lastNum = 0;
		
		do {
			if(c == '[') {			
				if(current == NULL) {
					current = &input[lineCount][nodeCount];
					nodeCount++;
				}else{
					stack[stackPointer] = current;
					stackPointer++;
					current = &input[lineCount][nodeCount];
					nodeCount++;
				}
			}else if(c == ']') {
				if(lastNode != NULL) {
					current->rightNode = lastNode;
				}else{
					current->rightNumber = lastNum;
					lastNum = 0;
				}
			
				lastNode = current;
				stackPointer--;
				if(stackPointer < 0) {
					break;
				}
				current = stack[stackPointer];
			}else if(c == ',') {
				if(lastNode != NULL) {
					current->leftNode = lastNode;
				}else{
					current->leftNumber = lastNum;
					lastNum = 0;
				}
			}else if(c >= '0' && c <= '9') {
				lastNode = NULL;
				int num = c - '0';
				lastNum *= 10;
				lastNum += num;
			}else{
				printf("Unexpected character %c, skipping\n", c);
			}
			
			i++;
			if(i >= maxLineLength) {
				printf("Read Line Too Long\n");
				return 1;
			}
			c = buffer[i];
		} while(buffer[i] != '\0' && buffer[i] != '\n');
		
		lineCount++;
		printf("Found %d nodes in line %d.\n", nodeCount, lineCount);
		printf("%s\n", buffer);
		printNumber(input[lineCount-1], true);
	}

	printf("Found %d lines in %s.\n", lineCount, filename);
	
	if(lineCount == 1) {
		printf("Reducing single line\n");
		int rr = reduce(input[0]);
		while(rr > 0) {
			rr = reduce(input[0]);
		}	
		printNumber(input[0], true);
	}else{
		int maxMagnitude = 0;
		
		struct node *left = malloc(128 * sizeof(struct node));
		struct node *right = malloc(128 * sizeof(struct node));
		
		for(int i=0; i<lineCount; i++) {
			for(int j=0; j<lineCount; j++) {
				if(i == j) {
					continue;
				}
				
				struct node sum;
				
				memcpy(left, input[i], 128 * sizeof(struct node));
				memcpy(right, input[j], 128 * sizeof(struct node));
				
				// Update pointers on copy
				for(int p=0; p<128; p++) {
					struct node *l = &left[p];
					struct node *r = &right[p];
					
					if(l->leftNode != NULL) {
						l->leftNode = left + (l->leftNode - input[i]);
					}
					if(l->rightNode != NULL) {
						l->rightNode = left + (l->rightNode - input[i]);
					}
					if(r->leftNode != NULL) {
						r->leftNode = right + (r->leftNode - input[j]);
					}
					if(r->rightNode != NULL) {
						r->rightNode = right + (r->rightNode - input[j]);
					}
				}
				
				sum.leftNode = left;
				sum.rightNode = right;
				
				int rr = reduce(&sum);
				while(rr > 0) {
					rr = reduce(&sum);
				}
				int m = magnitude(&sum);
				
				if(m > maxMagnitude) {
					/*printNumber(input[i], true);
					printf("+\n");
					printNumber(input[j], true);
					printf("=\n");
					printNumber(&sum, true);
					printf("m = %d\n\n", m);*/
					maxMagnitude = m;
				}
				
				/*printNumber(input[i], true);
				printf("+\n");
				printNumber(input[j], true);
				printf("=\n");
				printNumber(&sum, true);
				printf("%d + %d m = %d\n\n", i, j, m);*/
				
				memcpy(left, input[i], 128 * sizeof(struct node));
				memcpy(right, input[j], 128 * sizeof(struct node));
				
				// Update pointers on copy
				for(int p=0; p<128; p++) {
					struct node *l = &left[p];
					struct node *r = &right[p];
					
					if(l->leftNode != NULL) {
						l->leftNode = left + (l->leftNode - input[i]);
					}
					if(l->rightNode != NULL) {
						l->rightNode = left + (l->rightNode - input[i]);
					}
					if(r->leftNode != NULL) {
						r->leftNode = right + (r->leftNode - input[j]);
					}
					if(r->rightNode != NULL) {
						r->rightNode = right + (r->rightNode - input[j]);
					}
				}
				
				sum.leftNode = right;
				sum.rightNode = left;
				
				rr = reduce(&sum);
				while(rr > 0) {
					rr = reduce(&sum);
				}
				m = magnitude(&sum);
				
				if(m > maxMagnitude) {
					/*printNumber(input[j], true);
					printf("+\n");
					printNumber(input[i], true);
					printf("=\n");
					printNumber(&sum, true);
					printf("m = %d\n\n", m);*/
					maxMagnitude = m;
				}
				
				/*printNumber(input[j], true);
				printf("+\n");
				printNumber(input[i], true);
				printf("=\n");
				printNumber(&sum, true);
				printf("%d + %d m = %d\n\n", j, i, m);*/
			}
		}
		
		printf("Max magnitude is %d\n", maxMagnitude);
	}

	fclose(file);

	return 0;
}
