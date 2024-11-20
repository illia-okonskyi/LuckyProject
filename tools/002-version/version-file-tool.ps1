class LpAesProperties {
    [string] $Key
    [string] $IV

    LpAesProperties([string] $key, [string] $iv) {
        $this.Key = $key;
        $this.IV = $iv;
    }
}

class LpVersionFileTool {
    static [LpAesProperties] ReadAesPropertiesJsonFile([string]$path) {
        $json = [System.IO.File]::ReadAllText($path) | ConvertFrom-Json;
        return [LpAesProperties]::new($json.Key, $json.IV);
    }

    static [void] WriteVersionFile(
        [string] $version,
        [bool] $encrypt,
        [string] $path,
        [LpAesProperties] $aesProps) {

        if ($encrypt -eq $false) {
            [System.IO.File]::WriteAllText($path, $version);
            return;
        }
    
        $aes = [LpVersionFileTool]::_CreateAes($aesProps); 
        $encryptor = $aes.CreateEncryptor();
        $ms = New-Object -TypeName IO.MemoryStream;
        $cs = New-Object -TypeName Security.Cryptography.CryptoStream `
          -ArgumentList @($ms, $encryptor, 'Write' );
          
        $versionBytes = [Text.Encoding]::UTF8.GetBytes($version);
        $cs.Write($versionBytes, 0, $versionBytes.Length);
        $cs.FlushFinalBlock();
        $encryptedBytes = $ms.ToArray();
        $encryptedVersion = [Convert]::ToBase64String($encryptedBytes);    
        [System.IO.File]::WriteAllText($path, $encryptedVersion);
    }
    
    static [string] ReadVersionFile(
        [string] $path,
        [bool] $isEncrypted,
        [LpAesProperties] $aesProps) {

        if ($isEncrypted -eq $false) {
            return [System.IO.File]::ReadAllText($path);
        }
    
        $aes = [LpVersionFileTool]::_CreateAes($aesProps); 
        $decryptor = $aes.CreateDecryptor();
        $ms = New-Object -TypeName IO.MemoryStream;
        $cs = New-Object -TypeName Security.Cryptography.CryptoStream `
          -ArgumentList @($ms, $decryptor, 'Write' );
    
        $encryptedVersion = [System.IO.File]::ReadAllText($path);
        $encryptedBytes = [Convert]::FromBase64String($encryptedVersion);
        $cs.Write($encryptedBytes, 0, $encryptedBytes.Length);
        $cs.FlushFinalBlock();
        $versionBytes = $ms.ToArray()
        return [Text.Encoding]::UTF8.GetString($versionBytes);    
    }

    hidden static [Security.Cryptography.Aes] _CreateAes(
        [LpAesProperties] $aesProps) {
        $aes = [Security.Cryptography.Aes]::Create();
        $aes.Key = [Convert]::FromBase64String($aesProps.Key);
        $aes.IV = [Convert]::FromBase64String($aesProps.IV);
        return $aes;
    }   
}
