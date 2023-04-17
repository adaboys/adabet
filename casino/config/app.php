<?php
/**
 *   Stake iGaming platform
 *   ----------------------
 *   app.php
 * 
 *   @copyright  Copyright (c) FinancialPlugins, All rights reserved
 *   @author     FinancialPlugins <info@financialplugins.com>
 *   @see        https://financialplugins.com
*/

return [

    'version' => '1.19.0',

    

    'name' => env('APP_NAME', 'Stake'),

    

    'logo' => env('APP_LOGO', '/images/logo/logo.png'),

    

    'env' => env('APP_ENV', 'production'),

    

    'debug' => (bool) env('APP_DEBUG', false),

    

    'url' => env('APP_URL', ''),

    'asset_url' => env('ASSET_URL', null),

    'force_ssl' => env('FORCE_SSL', FALSE),

    

    'timezone' => 'UTC',

    

    'locale' => env('LOCALE', 'en'),

    
    
    'default_locale' => env('LOCALE', 'en'),

    'detect_browser_locale' => env('DETECT_BROWSER_LOCALE', TRUE),

    'locales' => [
        'en' => [
            'flag'  => 'us',
            'title' => 'English'
        ],
        'ru' => [
            'title' => 'Русский'
        ],
        'de' => [
            'title' => 'Deutsch',
        ],
        'es' => [
            'title' => 'Español',
        ],
        'fr' => [
            'title' => 'Français',
        ],
        'pt' => [
            'title' => 'Português',
        ],
        'nl' => [
            'title' => 'Nederlands',
        ],
        'cs' => [
            'flag'  => 'cz',
            'title' => 'Česky',
        ],
        'it' => [
            'title' => 'Italiano',
        ],
        'fi' => [
            'title' => 'Suomi',
        ],
        'sv' => [
            'flag'  => 'se',
            'title' => 'Svenska',
        ],
        'hu' => [
            'title' => 'Magyar',
        ],
        'el' => [
            'flag'  => 'gr',
            'title' => 'Ελληνικά',
        ],
        'da' => [
            'flag'  => 'dk',
            'title' => 'Dansk',
        ],
        'lv' => [
            'title' => 'Latviešu',
        ],
        'lt' => [
            'title' => 'Lietuvių',
        ],
        'et' => [
            'flag'  => 'ee',
            'title' => 'Eesti',
        ],
        'sk' => [
            'title' => 'Slovenčina',
        ],
        'sl' => [
            'flag'  => 'si',
            'title' => 'Slovenščina',
        ],
        'ko' => [
            'flag'  => 'kr',
            'title' => '한국어',
        ],
        
        'zh-cn' => [
            'flag'  => 'cn',
            'title' => '简体',
        ],
        
        'zh-tw' => [
            'flag'  => 'tw',
            'title' => '繁体',
        ],
        'ja' => [
            'flag'  => 'jp',
            'title' => '日本語',
        ],
    ],

    'translation_files_folder' => env('TRANSLATION_FILES_FOLDER', 'lang'),

    

    'fallback_locale' => 'en',

    

    'faker_locale' => 'en_US',

    

    'key' => env('APP_KEY'),

    'cipher' => 'AES-256-CBC',

    'hash' => 'da34b0aa975fddd5a90a1eb2bb8cb9d2',

    'hashb' => '',

    

    'api' => [
        'releases' => [
            'base_url' => env('API_RELEASES_BASE_URL', 'https://stake.financialplugins.com/api/')
        ],
        'products' => [
            'base_url' => env('API_PRODUCTS_BASE_URL', 'https://financialplugins.com/api/')
        ],
    ],

    

    'providers' => [

        
        Illuminate\Auth\AuthServiceProvider::class,
        Illuminate\Broadcasting\BroadcastServiceProvider::class,
        Illuminate\Bus\BusServiceProvider::class,
        Illuminate\Cache\CacheServiceProvider::class,
        Illuminate\Foundation\Providers\ConsoleSupportServiceProvider::class,
        Illuminate\Cookie\CookieServiceProvider::class,
        Illuminate\Database\DatabaseServiceProvider::class,
        Illuminate\Encryption\EncryptionServiceProvider::class,
        Illuminate\Filesystem\FilesystemServiceProvider::class,
        Illuminate\Foundation\Providers\FoundationServiceProvider::class,
        Illuminate\Hashing\HashServiceProvider::class,
        Illuminate\Mail\MailServiceProvider::class,
        Illuminate\Notifications\NotificationServiceProvider::class,
        Illuminate\Pagination\PaginationServiceProvider::class,
        Illuminate\Pipeline\PipelineServiceProvider::class,
        Illuminate\Queue\QueueServiceProvider::class,
        Illuminate\Redis\RedisServiceProvider::class,
        Illuminate\Auth\Passwords\PasswordResetServiceProvider::class,
        Illuminate\Session\SessionServiceProvider::class,
        Illuminate\Translation\TranslationServiceProvider::class,
        Illuminate\Validation\ValidationServiceProvider::class,
        Illuminate\View\ViewServiceProvider::class,

        
        SocialiteProviders\Manager\ServiceProvider::class,

        
        App\Providers\AppServiceProvider::class,
        App\Providers\AuthServiceProvider::class,
        App\Providers\BroadcastServiceProvider::class,
        App\Providers\EventServiceProvider::class,
        App\Providers\RouteServiceProvider::class,

    ],

    

    'aliases' => [

        'App' => Illuminate\Support\Facades\App::class,
        'Arr' => Illuminate\Support\Arr::class,
        'Artisan' => Illuminate\Support\Facades\Artisan::class,
        'Auth' => Illuminate\Support\Facades\Auth::class,
        'Blade' => Illuminate\Support\Facades\Blade::class,
        'Broadcast' => Illuminate\Support\Facades\Broadcast::class,
        'Bus' => Illuminate\Support\Facades\Bus::class,
        'Cache' => Illuminate\Support\Facades\Cache::class,
        'Config' => Illuminate\Support\Facades\Config::class,
        'Cookie' => Illuminate\Support\Facades\Cookie::class,
        'Crypt' => Illuminate\Support\Facades\Crypt::class,
        'DB' => Illuminate\Support\Facades\DB::class,
        'Eloquent' => Illuminate\Database\Eloquent\Model::class,
        'Event' => Illuminate\Support\Facades\Event::class,
        'File' => Illuminate\Support\Facades\File::class,
        'Gate' => Illuminate\Support\Facades\Gate::class,
        'Hash' => Illuminate\Support\Facades\Hash::class,
        'Http' => Illuminate\Support\Facades\Http::class,
        'Lang' => Illuminate\Support\Facades\Lang::class,
        'Log' => Illuminate\Support\Facades\Log::class,
        'Mail' => Illuminate\Support\Facades\Mail::class,
        'Notification' => Illuminate\Support\Facades\Notification::class,
        'Password' => Illuminate\Support\Facades\Password::class,
        'Queue' => Illuminate\Support\Facades\Queue::class,
        'Redirect' => Illuminate\Support\Facades\Redirect::class,
        
        'Request' => Illuminate\Support\Facades\Request::class,
        'Response' => Illuminate\Support\Facades\Response::class,
        'Route' => Illuminate\Support\Facades\Route::class,
        'Schema' => Illuminate\Support\Facades\Schema::class,
        'Session' => Illuminate\Support\Facades\Session::class,
        'Storage' => Illuminate\Support\Facades\Storage::class,
        'Str' => Illuminate\Support\Str::class,
        'URL' => Illuminate\Support\Facades\URL::class,
        'Validator' => Illuminate\Support\Facades\Validator::class,
        'View' => Illuminate\Support\Facades\View::class,

    ],

    
    'debug_blacklist' => [
        '_ENV' => [
            'APP_KEY',
            'DB_DATABASE',
            'DB_USERNAME',
            'DB_PASSWORD',
            'REDIS_URL',
            'REDIS_HOST',
            'REDIS_PASSWORD',
            'REDIS_PORT',
            'MAIL_HOST',
            'MAIL_USERNAME',
            'MAIL_PASSWORD',
            'PUSHER_APP_SECRET',
            'PURCHASE_CODE',
            'AMERICAN_ROULETTE_PURCHASE_CODE',
            'BLACKJACK_PURCHASE_CODE',
            'CARIBBEAN_POKER_PURCHASE_CODE',
            'CASINO_HOLDEM_PURCHASE_CODE',
            'CRASH_PURCHASE_CODE',
            'CRYPTO_PREDICTION_PURCHASE_CODE',
            'DICE_PURCHASE_CODE',
            'DICE_3D_PURCHASE_CODE',
            'EUROPEAN_ROULETTE_PURCHASE_CODE',
            'HEADS_OR_TAILS_PURCHASE_CODE',
            'HORSE_RACING_PURCHASE_CODE',
            'KENO_PURCHASE_CODE',
            'LUCKY_WHEEL_PURCHASE_CODE',
            'MULTIPLAYER_BLACKJACK_PURCHASE_CODE',
            'MULTIPLAYER_ROULETTE_PURCHASE_CODE',
            'RAFFLE_PURCHASE_CODE',
            'PLINKO_PURCHASE_CODE',
            'SIC_BO_PURCHASE_CODE',
            'SLOTS_PURCHASE_CODE',
            'SLOTS_3D_PURCHASE_CODE',
            'VIDEO_POKER_PURCHASE_CODE',
            'AMERICAN_ROULETTE_SECURITY_HASH',
            'BLACKJACK_SECURITY_HASH',
            'CARIBBEAN_POKER_SECURITY_HASH',
            'CASINO_HOLDEM_SECURITY_HASH',
            'CRASH_SECURITY_HASH',
            'CRYPTO_PREDICTION_SECURITY_HASH',
            'DICE_SECURITY_HASH',
            'DICE_3D_SECURITY_HASH',
            'EUROPEAN_ROULETTE_SECURITY_HASH',
            'HEADS_OR_TAILS_SECURITY_HASH',
            'HORSE_RACING_SECURITY_HASH',
            'KENO_SECURITY_HASH',
            'LUCKY_WHEEL_SECURITY_HASH',
            'MULTIPLAYER_BLACKJACK_SECURITY_HASH',
            'MULTIPLAYER_ROULETTE_SECURITY_HASH',
            'RAFFLE_SECURITY_HASH',
            'PLINKO_SECURITY_HASH',
            'SIC_BO_SECURITY_HASH',
            'SLOTS_SECURITY_HASH',
            'SLOTS_3D_SECURITY_HASH',
            'VIDEO_POKER_SECURITY_HASH',
            'LICENSEE_EMAIL',
            'SECURITY_HASH',
            'FACEBOOK_CLIENT_SECRET',
            'TWITTER_CLIENT_SECRET',
            'LINKEDIN_CLIENT_SECRET',
            'GOOGLE_CLIENT_SECRET',
            'YAHOO_CLIENT_SECRET',
            'COINBASE_CLIENT_SECRET',
            'STEEM_CLIENT_SECRET',
            'RECAPTCHA_SECRET_KEY',
            'PAYMENTS_PAYPAL_USER',
            'PAYMENTS_PAYPAL_PASSWORD',
            'PAYMENTS_PAYPAL_SIGNATURE',
            'PAYMENTS_SKRILL_ACCOUNT_EMAIL',
            'PAYMENTS_SKRILL_SECRET_WORD',
            'PAYMENTS_STRIPE_PUBLIC_KEY',
            'PAYMENTS_STRIPE_SECRET_KEY',
            'PAYMENTS_COINPAYMENTS_MERCHANT_ID',
            'PAYMENTS_COINPAYMENTS_PUBLIC_KEY',
            'PAYMENTS_COINPAYMENTS_PRIVATE_KEY',
            'PAYMENTS_COINPAYMENTS_SECRET_KEY',
            'PAYMENTS_CRYPTAPI_SECRET',
            'PAYMENTS_FREEKASSA_API_KEY',
            'PAYMENTS_FREEKASSA_SECRET_WORD',
            'PAYMENTS_FREEKASSA_SECRET_WORD2',
            'PAYMENTS_FREEKASSA_MERCHANT_ID',
            'PAYMENTS_PAYTM_MERCHANT_ID',
            'PAYMENTS_PAYTM_SECRET_KEY',
            'PAYMENTS_MERCADOPAGO_ACCESS_TOKEN',
            'PAYMENTS_SMARTFASTPAY_CLIENT_ID',
            'PAYMENTS_SMARTFASTPAY_SECRET_KEY',
            'PAYMENTS_EASYWIRE_USER',
            'PAYMENTS_EASYWIRE_PASSWORD',
            'API_CRYPTO_PROVIDERS_CRYPTOCOMPARE_API_KEY',
            'RECAPTCHA_SECRET_KEY',
            'GAME_PROVIDERS_BGAMING_API_ID',
            'GAME_PROVIDERS_BGAMING_API_SECRET_KEY',
            'GAME_PROVIDERS_EVOPLAY_API_ID',
            'GAME_PROVIDERS_EVOPLAY_API_SECRET_KEY',
            'GAME_PROVIDERS_KAGAMING_API_ID',
            'GAME_PROVIDERS_KAGAMING_API_ACCESS_KEY',
            'GAME_PROVIDERS_KAGAMING_API_ACCESS_KEY',
            'GAME_PROVIDERS_KAGAMING_API_ENDPOINT',
            'GAME_PROVIDERS_KAGAMING_API_GAME_LAUNCH_URL',
        ],
        '_SERVER' => [
            'APP_KEY',
            'DB_DATABASE',
            'DB_USERNAME',
            'DB_PASSWORD',
            'REDIS_URL',
            'REDIS_HOST',
            'REDIS_PASSWORD',
            'REDIS_PORT',
            'MAIL_HOST',
            'MAIL_USERNAME',
            'MAIL_PASSWORD',
            'PUSHER_APP_SECRET',
            'PURCHASE_CODE',
            'AMERICAN_ROULETTE_PURCHASE_CODE',
            'BLACKJACK_PURCHASE_CODE',
            'CARIBBEAN_POKER_PURCHASE_CODE',
            'CASINO_HOLDEM_PURCHASE_CODE',
            'CRASH_PURCHASE_CODE',
            'CRYPTO_PREDICTION_PURCHASE_CODE',
            'DICE_PURCHASE_CODE',
            'DICE_3D_PURCHASE_CODE',
            'EUROPEAN_ROULETTE_PURCHASE_CODE',
            'HEADS_OR_TAILS_PURCHASE_CODE',
            'HORSE_RACING_PURCHASE_CODE',
            'KENO_PURCHASE_CODE',
            'LUCKY_WHEEL_PURCHASE_CODE',
            'MULTIPLAYER_BLACKJACK_PURCHASE_CODE',
            'MULTIPLAYER_ROULETTE_PURCHASE_CODE',
            'RAFFLE_PURCHASE_CODE',
            'PLINKO_PURCHASE_CODE',
            'SIC_BO_PURCHASE_CODE',
            'SLOTS_PURCHASE_CODE',
            'SLOTS_3D_PURCHASE_CODE',
            'VIDEO_POKER_PURCHASE_CODE',
            'AMERICAN_ROULETTE_SECURITY_HASH',
            'BLACKJACK_SECURITY_HASH',
            'CARIBBEAN_POKER_SECURITY_HASH',
            'CASINO_HOLDEM_SECURITY_HASH',
            'CRASH_SECURITY_HASH',
            'CRYPTO_PREDICTION_SECURITY_HASH',
            'DICE_SECURITY_HASH',
            'DICE_3D_SECURITY_HASH',
            'EUROPEAN_ROULETTE_SECURITY_HASH',
            'HEADS_OR_TAILS_SECURITY_HASH',
            'HORSE_RACING_SECURITY_HASH',
            'KENO_SECURITY_HASH',
            'LUCKY_WHEEL_SECURITY_HASH',
            'MULTIPLAYER_BLACKJACK_SECURITY_HASH',
            'MULTIPLAYER_ROULETTE_SECURITY_HASH',
            'RAFFLE_SECURITY_HASH',
            'PLINKO_SECURITY_HASH',
            'SIC_BO_SECURITY_HASH',
            'SLOTS_SECURITY_HASH',
            'SLOTS_3D_SECURITY_HASH',
            'VIDEO_POKER_SECURITY_HASH',
            'LICENSEE_EMAIL',
            'SECURITY_HASH',
            'FACEBOOK_CLIENT_SECRET',
            'TWITTER_CLIENT_SECRET',
            'LINKEDIN_CLIENT_SECRET',
            'GOOGLE_CLIENT_SECRET',
            'YAHOO_CLIENT_SECRET',
            'COINBASE_CLIENT_SECRET',
            'STEEM_CLIENT_SECRET',
            'RECAPTCHA_SECRET_KEY',
            'PAYMENTS_PAYPAL_USER',
            'PAYMENTS_PAYPAL_PASSWORD',
            'PAYMENTS_PAYPAL_SIGNATURE',
            'PAYMENTS_SKRILL_ACCOUNT_EMAIL',
            'PAYMENTS_SKRILL_SECRET_WORD',
            'PAYMENTS_STRIPE_PUBLIC_KEY',
            'PAYMENTS_STRIPE_SECRET_KEY',
            'PAYMENTS_COINPAYMENTS_MERCHANT_ID',
            'PAYMENTS_COINPAYMENTS_PUBLIC_KEY',
            'PAYMENTS_COINPAYMENTS_PRIVATE_KEY',
            'PAYMENTS_COINPAYMENTS_SECRET_KEY',
            'PAYMENTS_CRYPTAPI_SECRET',
            'PAYMENTS_FREEKASSA_API_KEY',
            'PAYMENTS_FREEKASSA_SECRET_WORD',
            'PAYMENTS_FREEKASSA_SECRET_WORD2',
            'PAYMENTS_FREEKASSA_MERCHANT_ID',
            'PAYMENTS_PAYTM_MERCHANT_ID',
            'PAYMENTS_PAYTM_SECRET_KEY',
            'PAYMENTS_MERCADOPAGO_ACCESS_TOKEN',
            'PAYMENTS_SMARTFASTPAY_CLIENT_ID',
            'PAYMENTS_SMARTFASTPAY_SECRET_KEY',
            'PAYMENTS_EASYWIRE_USER',
            'PAYMENTS_EASYWIRE_PASSWORD',
            'API_CRYPTO_PROVIDERS_CRYPTOCOMPARE_API_KEY',
            'RECAPTCHA_SECRET_KEY',
            'GAME_PROVIDERS_BGAMING_API_ID',
            'GAME_PROVIDERS_BGAMING_API_SECRET_KEY',
            'GAME_PROVIDERS_EVOPLAY_API_ID',
            'GAME_PROVIDERS_EVOPLAY_API_SECRET_KEY',
            'GAME_PROVIDERS_KAGAMING_API_ID',
            'GAME_PROVIDERS_KAGAMING_API_ACCESS_KEY',
            'GAME_PROVIDERS_KAGAMING_API_ACCESS_KEY',
            'GAME_PROVIDERS_KAGAMING_API_ENDPOINT',
            'GAME_PROVIDERS_KAGAMING_API_GAME_LAUNCH_URL',
        ],
    ],
];
