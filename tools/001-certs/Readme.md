# Generate self-seigned certs with own Root CA for usage in .NET Kestrel

- Generate private key to become a local certificate authority:
`openssl genrsa -des3 -out localCA.key 2048`
- Output from above command creates private key: `localCA.key`.
- Generate root certificate using above private key:
`openssl req -x509 -new -nodes -key localCA.key -sha256 -days 365 -out localCA.crt`
- Output from above command creates root certificate: `localCA.crt`.
- You have created your local Root CA Authority, outputs are:
    - `localCA.key` - private key for your local Root CA
    - `localCA.crt` - certificate of your local Root CA, uou must add this certificate as trusted root
CA to your host/targets.
- Now you can create certs & keys signed by your local root CA
- Create a private key: `openssl genrsa -out cert.key 2048`
- Output from above command creates private key: `cert.key`.
- Create a CSR (Certificate Signing Request) for private key `cert.key`:
`openssl req -new -key cert.key -out cert.csr`
- Using any text editor create a cert extension file `cert.ext` and include below information in
the file:
```
authorityKeyIdentifier=keyid,issuer
basicConstraints=CA:FALSE
keyUsage = digitalSignature, nonRepudiation, keyEncipherment, dataEncipherment
subjectAltName = @alt_names
[alt_names]
DNS.1 = <enter-DNS1>
```
- Create a self signed SSL cert using `cert.csr`, local root certificate `localCA.crt`,
local certificate authority private key `localCA.key`:
```
openssl x509 -req -in cert.csr -CA localCA.crt -CAkey localCA.key -CAcreateserial \
-out cert.crt -days 365 -sha256 -extfile cert.ext
```
- Output from above command creates self signed SSL cert: `cert.crt`.
- Create PFX file: 
`openssl pkcs12 -export -out cert.pfx -inkey cert.key -in cert.crt`
- Outputs are:
    - `cert.key` - certificate private key;
    - `cert.crt` - self-signed certificate;
    - `cert.pfx` - PKCS-12 archive which includes self-signed certificate and key