c = get_config()
c.NotebookApp.extra_static_paths = [ r"C:\ProgramData\jupyter\kernels\csharp\static" ]
c.update('notebook', {"CodeCell": {"cm_config": {"autoCloseBrackets": False}}})
