pipeline {
    agent any

    environment {
        DOCKER_IMAGE = 'heatbalance-frontend'
        DOCKER_TAG = "${env.BUILD_NUMBER}"
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Install Dependencies') {
            steps {
                dir('frontend') {
                    sh 'npm ci --silent'
                }
            }
        }

        stage('Lint') {
            steps {
                dir('frontend') {
                    sh 'npm run lint || true'
                }
            }
        }

        stage('Build') {
            steps {
                dir('frontend') {
                    sh 'npm run build'
                }
            }
        }

        stage('Docker Build') {
            when {
                branch 'main'
            }
            steps {
                dir('frontend') {
                    sh "docker build -t ${DOCKER_IMAGE}:${DOCKER_TAG} -t ${DOCKER_IMAGE}:latest ."
                }
            }
        }

        stage('Deploy') {
            when {
                branch 'main'
            }
            steps {
                sh "docker compose up -d frontend"
            }
        }
    }

    post {
        success {
            echo '✅ Frontend build and deploy completed successfully!'
        }
        failure {
            echo '❌ Frontend build failed.'
        }
        always {
            cleanWs()
        }
    }
}
