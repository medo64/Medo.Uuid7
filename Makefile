.PHONY: all clean distclean dist debug release package test

all: release

clean:
	@./Make.sh clean

distclean: clean
	@./Make.sh distclean

dist:
	@./Make.sh dist

debug:
	@./Make.sh debug

release:
	@./Make.sh release

package:
	@./Make.sh package

test: debug
	@./Make.sh test
