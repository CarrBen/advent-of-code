CFLAGS += -Wall -g
LIBS += -lm

all: p1 p2

p1: p1.o
	$(CC) $(CFLAGS) p1.o -o p1 $(LIBS)

p2: p2.o
	$(CC) $(CFLAGS) p2.o -o p2 $(LIBS)

p1.o: p1.c
	$(CC) $(CFLAGS) -c p1.c

p2.o: p2.c
	$(CC) $(CFLAGS) -c p2.c

clean:
	rm -f *.o
	rm -f p1
	rm -f p2
