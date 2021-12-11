#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <unistd.h>
#include <fcntl.h>
#include <sys/sendfile.h>

int copyFile(const char* src, const char* dest)
{
	int fdIn, fdOut;
	if ((fdIn = open(src, O_RDONLY)) == -1)
	{
		perror("Open src failed");
		return -1;
	}

	if ((fdOut = creat(dest, 0660)) == -1)
	{
		close(fdIn);
		perror("Open dest failed");
		return -1;
	}

	off_t bytesCopied = 0;
	struct stat fileInfo = {0};
	fstat(fdIn, &fileInfo);
	int result = sendfile(fdOut, fdIn, &bytesCopied, fileInfo.st_size);

	close(fdIn);
	close(fdOut);

	return result;
}

int main(int argc, char* argv[])
{
	int dayNumber;
	sscanf(argv[1], "%d", &dayNumber);

	struct stat st = {0};

	char dirName[6];
	sprintf(dirName, "day%d", dayNumber);

	if (stat(dirName, &st) == -1) {
		mkdir(dirName, 0700);
	}

	printf("Created directory for day %d.\n", dayNumber);

	char* files[] = { "Makefile.template", "p1.c.template", "p2.c.template" };
	int fileCount = sizeof(files)/sizeof(files[0]);

	for (int i=0; i<fileCount; i++)
	{
		char srcName[100];
		strcpy(srcName, "template/");
		strcat(srcName, files[i]);
		char destName[100];
		destName[0] = '\0';
		memset(destName, 0, sizeof(destName));
		strcpy(destName, dirName);
		strcat(destName, "/");
		char destFileName[100];
		destFileName[0] = '\0';
		memset(destFileName, 0, sizeof(destFileName));
		strncpy(destFileName, files[i], strlen(files[i]) - 9); // 9 for ".template"
		strcat(destName, destFileName);
		printf("Copy %s to %s.\n", srcName, destName);
		copyFile(srcName, destName);
	}

	printf("Copied templates files for day %d.\n", dayNumber);

	return 0;
}
