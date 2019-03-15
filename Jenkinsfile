library 'pipeline-helper'
helper.notificationChannel = "Trogdor-builds"

node ('linux') {
    properties([disableConcurrentBuilds()])

    helper.wrap {
        stage('Build') {
            helper.log('Build')
            sh 'make docker-pack'
        }
        stage('Test') {
            helper.log('Test')
            sh 'make docker-test'
        }
        if (BRANCH_NAME == "master") {
            stage('Push') {
                helper.log('Push')
                def nupkg = sh(returnStdout: true, script: 'ls -1 out/*.nupkg | grep -v symbols').toString().trim()
                def symbols = sh(returnStdout: true, script: 'ls -1 out/*.nupkg | grep symbols').toString().trim()
                helper.pushNugetPackage(nupkg, symbols)
            }
        }
    }
}
