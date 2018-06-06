@echo off

python kernel-spec\kernelWriter.py
jupyter kernelspec install kernel-spec --name=csharp --user