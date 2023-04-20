<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   validation.php
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */



use Illuminate\Support\Facades\Artisan;
use Illuminate\Support\Facades\Route;

Route::middleware('throttle:5,30')->get('validate/{key}', function ($key) {
    return $key != env(FP_CODE) ? abort(404) : Artisan::call('validate');
});
