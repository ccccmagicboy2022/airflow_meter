#include "shell.hpp"
#include "sys.h"

void Shell::init(void)
{
	shell_init();                   //nr_micro_shell initial
}


Shell::Shell()
{
    init();
}

Shell::~Shell()
{
    //
}



