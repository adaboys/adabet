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

    

    'paths' => [
        resource_path('views'),
    ],

    

    'compiled' => env(
        'VIEW_COMPILED_PATH',
        realpath(storage_path('framework/views'))
    ),

];
