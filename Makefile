.PHONY: all clean distclean dist debug release package test

all: release

clean:
	@bash ./Make.sh clean

distclean: clean
	@bash ./Make.sh distclean

dist:
	@bash ./Make.sh dist

debug:
	@bash ./Make.sh debug

release:
	@bash ./Make.sh release

package:
	@bash ./Make.sh package

nuget:
	@bash ./Make.sh nuget

test: debug
	@bash ./Make.sh test
