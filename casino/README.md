# Casino

Written in laravel 8.83


## Quickstart


## Edit logs

- Editted by darkcompet

	```bash
	# Create .env file
	cp .env.example .env

	# Modify .env
	APP_ENV=development

	DB_DATABASE=adabet_casino
	DB_USERNAME=root
	DB_PASSWORD=Local1234!
	
	# Generate app key
	php artisan key:generate
 
 	# Setting domain cookie
 	APP_URL=https://domain-casino.com
 	SANCTUM_STATEFUL_DOMAINS=domain-casino.com
	```
