from urllib.request import urlretrieve
import os
import zipfile


def install_python():
  print("Downloading Python 3.12.3...")
  urlretrieve('https://github.com/adang1345/PythonWin7/raw/master/3.12.3/python-3.12.3-full.exe', 'python3_installer.exe')
  
  print("Installing Python 3.12.3...")
  os.system("python3_installer.exe /passive AppendPath=0 PrependPath=0 DefaultAllUsersTargetDir=C:\\UDM-Python")

  print("Removing python3 installer")

  os.remove("python3_installer.exe")

def install_mtkclient():
  
  print("Downloading mtkclient from master branch...")
  urlretrieve('https://github.com/bkerler/mtkclient/archive/refs/heads/main.zip', 'mtkclient.zip')

  print("Extracting mtkclient to 'mtkclient' folder")
  with zipfile.ZipFile('mtkclient.zip', 'r') as zip_ref:
    zip_ref.extractall('mtkclient')

  print('Installing dependences')
  install_requirements="%LocalAppData%\Programs\Python\Python312-32\python.exe -m pip install -r " + str(os.path.abspath(os.getcwd())) + "\\mtkclient\\mtkclient-main\\requirements.txt"
  os.system(install_requirements)


if __name__ == "__main__":
  install_python()
  install_mtkclient()
  
