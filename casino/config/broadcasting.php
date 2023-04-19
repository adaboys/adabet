<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   config
 * 
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
*/

return [

    

    'default' => env('BROADCAST_DRIVER', 'pusher'),

    

    'connections' => [

        'pusher' => [
            'driver' => 'pusher',
            'app_id' => env('PUSHER_APP_ID'),
            'key' => env('PUSHER_APP_KEY'),
            'secret' => env('PUSHER_APP_SECRET'),
            'options' => [
                'cluster' => env('PUSHER_APP_CLUSTER', 'eu'),
                'encrypted' => TRUE,
                'useTLS' => TRUE,
            ],
        ],

        'redis' => [
            'driver' => 'redis',
            'connection' => 'default',
        ],

        'log' => [
            'driver' => 'log',
        ],

        'null' => [
            'driver' => 'null',
        ],

    ],

];
