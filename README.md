# Play.Identity
Play Economy Identity microservice

## Create and publish package
```powershell
$version="1.0.5"
$owner="dotNet-Microservices-Cource"
$gh_pat="[PAT HERE]"

dotnet pack src\Play.Identity.Contracts\ --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/play.identity -o..\packages

dotnet nuget push ..\packages\Play.Identity.Contracts.$version.nupkg --api-key $gh_pat --source "github"
```

## Build the docker image
```powershell
$env:GH_OWNER="dotNet-Microservices-Cource"
$env:GH_PAT="[PAT HERE]"
$appname="playeconomyapp"
docker build --secret id=GH_OWNER --secret id=GH_PAT -t "$appname.azurecr.io/play.identity:$version" .
```

## Run the docker image
```powershell
$adminPass="[PASSWORD HERE]"
$cosmosDbConnString="[CONN STRING HERE]"
$serviceBusConnString="[CONN STRING HERE]"
docker run -it --rm -p 5002:5002 --name identity -e MongoDbSettings__ConnectionString=$cosmosDbConnString -e ServiceBusSettings__ConnectionString=$serviceBusConnString -e ServiceSettings__MessageBroker="SERVICEBUS" -e IdentitySettings__AdminUserPassword=$adminPass play.identity:$version
```

## Publishing the Docker image
```powershell
az acr login --name $appname
docker push "$appname.azurecr.io/play.identity:$version"
```

## Create the Kubernetes namespace
```powershell
$namespace="identity"
kubectl create namespace $namespace
```

## Create the Kubernetes secrets
```powershell
kubectl create secret generic identity-secrets --from-literal=cosmosdb-connectionstring=$cosmosDbConnString --from-literal=servicebus-connectionstring=$serviceBusConnString --from-literal=admin-password-connectionstring=$adminPass -n $namespace
```

## Create the Kubernetes pod
```powershell
kubectl apply -f .\kubernetes\identity.yaml -n $namespace
```

## Creating the pod managed identity
```powershell
$resourcegroup="playeconomy"
az identity create --resource-group $resourcegroup --name $namespace
$IDENTITY_RESOURCE_ID=az identity show -g $resourcegroup -n $namespace --query id -otsv

az aks pod-identity add --resource-group $resourcegroup --cluster-name $appname --namespace $namespace --name $namespace --identity-resource-id $IDENTITY_RESOURCE_ID
```

## Granting access to Key Vault secrets
```powershell
$IDENTITY_CLIENT_ID=az identity show -g $resourcegroup -n $namespace --query clientId -otsv
az keyvault set-policy -n $appname --secret-permissions get list --spn $IDENTITY_CLIENT_ID
```