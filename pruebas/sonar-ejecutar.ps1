param([Parameter(Mandatory=$true)][string]$Stage)
$ErrorActionPreference='Stop'
$base='http://127.0.0.1:9002'
$admin=[pscredential]::new('admin',(ConvertTo-SecureString 'admin' -AsPlainText -Force))
$tokenName='lab-'+[guid]::NewGuid().ToString('N')
$projectRoot=(Resolve-Path 'TechRent-distribuido').Path
$scanScript=(Resolve-Path 'pruebas/sonar-scan.sh').Path
try {
 $token=(Invoke-RestMethod -Uri "$base/api/user_tokens/generate" -Method Post -Authentication Basic -Credential $admin -AllowUnencryptedAuthentication -Body @{name=$tokenName; type='GLOBAL_ANALYSIS_TOKEN'}).token
 $env:SONAR_TOKEN=$token
 docker run --rm --network techrent-sonar-lab -e SONAR_TOKEN -v "${projectRoot}:/src" -v "${scanScript}:/scan.sh:ro" -w /src --entrypoint /bin/sh mcr.microsoft.com/dotnet/sdk:10.0 /scan.sh 2>&1 | Set-Content -Encoding utf8 "_audit_work/sonar-$Stage.log"
 if($LASTEXITCODE -ne 0){throw "Scanner failed: $LASTEXITCODE"}
 $issue=Invoke-RestMethod -Uri "$base/api/issues/search?componentKeys=TechRentDistribuido&rules=csharpsquid%3AS3427&ps=500" -Authentication Basic -Credential $admin -AllowUnencryptedAuthentication
 $metrics=Invoke-RestMethod -Uri "$base/api/measures/component?component=TechRentDistribuido&metricKeys=ncloc,bugs,vulnerabilities,code_smells,security_hotspots,duplicated_lines_density,coverage" -Authentication Basic -Credential $admin -AllowUnencryptedAuthentication
 $gate=Invoke-RestMethod -Uri "$base/api/qualitygates/project_status?projectKey=TechRentDistribuido" -Authentication Basic -Credential $admin -AllowUnencryptedAuthentication
 @{issues=$issue;measures=$metrics;gate=$gate}|ConvertTo-Json -Depth 50|Set-Content -Encoding utf8 "_audit_work/sonar-$Stage.json"
 Write-Output "Stage=$Stage S3427=$($issue.total) Gate=$($gate.projectStatus.status)"
} finally {
 Remove-Item Env:SONAR_TOKEN -ErrorAction SilentlyContinue
 if($token){Invoke-RestMethod -Uri "$base/api/user_tokens/revoke" -Method Post -Authentication Basic -Credential $admin -AllowUnencryptedAuthentication -Body @{name=$tokenName}|Out-Null}
}
