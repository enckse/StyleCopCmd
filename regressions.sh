#!/bin/bash
SCC=$1
if [ ! -e "$SCC" ]; then
	exit 1
fi

BASEPATH="StyleCopCmd.Core.Test/data/StyleCopTestProject/"
OUTPUTXML="StyleCopTestViolations.xml"
FAILED="Failed"
SETTINGS=$BASEPATH"LocalSettings.Setting"
FILENAME=$BASEPATH"StyleCopTestProject/ClassOne.cs"
function test()
{
        echo $1
	RESULT=$(mono "$SCC" $1 $BASEPATH$2)
	MATCHES=$(echo $RESULT | grep -c "$3")
	if [ $MATCHES = "1" ]; then
		echo "Success"
	else
		exit 1
	fi
}

test "-s" "StyleCopTestProject.sln" "8 violations"
test "-p" "StyleCopTestProject/StyleCopTestProject.csproj" "8 violations"
test "-i Class -p" "StyleCopTestProject/StyleCopTestProject.csproj" "1 violations"
test "-d" "StyleCopTestProject/" "3 violations"
test "-r -d" "StyleCopTestProject/" "8 violations"
test "-f" "StyleCopTestProject/ClassOne.cs" "3 violations"
test "-v -f" "StyleCopTestProject/ClassOne.cs" "A closing curly bracket"
if [ -e "$OUTPUTXML" ]; then
	rm $OUTPUTXML
fi

test "-o $OUTPUTXML -f" "StyleCopTestProject/ClassOne.cs" "3 violations"
echo "-o"
if [ ! -e "$OUTPUTXML" ]; then
	exit 1
fi

test "-c $SETTINGS -r -d" "StyleCopTestProject/" "7 violations"
test "-e -f $FILENAME -f " "StyleCopTestProject/ClassOne.cs" "3 violations"

echo "-q"
RESULT=$(mono "$SCC" -q -r -d "$BASEPATH")
if [ "$RESULT" = "" ]; then
	echo "Success"
else
	exit 1
fi

test "-w -f" "StyleCopTestProject/ClassOne.cs" "Version Information"

echo "-?"
RESULT=$(mono "$SCC" -?)
MATCHES=$(echo $RESULT | grep -c "Provides a command line interface")
if [ $MATCHES == "1" ]; then
	echo "Success"
else
	exit 1
fi

echo "Normal exit"
mono $SCC -f $BASEPATH"StyleCopTestProject/ClassOne.cs"
if [ $? -eq 0 ]; then
	echo "Success"
else
	exit 1
fi

echo "Terminate exit"
mono $SCC -t -f $BASEPATH"StyleCopTestProject/ClassOne.cs"
if [ $? -eq 1 ]; then
	echo "Success"
else
	exit 1
fi

test "-x SOMEOTHER -r -d" "StyleCopTestProject/" "11 violations"
test "-u StyleCop -s" "StyleCopTestProject.sln" "No violations"
test "-u Style -s" "StyleCopTestProject.sln" "No violations"
test "-g Xml -r -d" "StyleCopTestProject/" "Violations were encountered"

