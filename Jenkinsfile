stage('Build Docker Image') {
    steps {
        script {
            docker.build("naruba200/mangawebapi:latest")
        }
    }
}

stage('Push to Docker Hub') {
    steps {
        script {
            docker.withRegistry('https://index.docker.io/v1/', 'dockerhub-credentials') {
                docker.image("naruba200/mangawebapi:latest").push()
            }
        }
    }
}
