# After downloading the StyleCop msi, unpack using 7z
# This doesn't handle multiple msi files but for now...whatever
7z e StyleCop-* -oc:\StyleCopMsi

# The MSI will produce not exactly dll names...
mv StyleCopMsi/AnalysisEngineDll StyleCopMsi/StyleCop.dll
mv StyleCopMsi/CSharpAnalyzer StyleCopMsi/StyleCop.CSharp.Rules.dll
mv StyleCopMsi/CSharpParser StyleCopMsi/StyleCop.CSharp.dll

mv StyleCopMsi/*.dll .
rm -rf StyleCopMsi