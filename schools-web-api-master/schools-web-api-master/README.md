# schools-web-api
THE BEST PRIVATE SCHOOLS WEB API ;)

In order to use add appsettings.json file must have below properties

"AllowedHosts": "*",
"ConnectionStrings": {
  "Postgres" : CONNECTION STRING TO YOUR POSTGRES DATABASE
},
"Jwt": {
  "Key": JWT SECKRET KEY
  "Issuer": JWT ISSUER
  "Audience": JWT AUDIENCE
  "Subject": JWT SUBJECT
}