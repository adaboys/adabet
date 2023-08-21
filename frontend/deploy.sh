echo "Deploy starting..."
source ~/.nvm/nvm.sh

npm install --omit=dev

rm -rf ../build
mv ../deploy ../build

pm2 startOrReload ecosystem.config.js --update-env

echo "Deploy done."