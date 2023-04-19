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

use Packages\Baccarat\Http\Controllers\GameController;


Route::name('games.baccarat.')
    ->middleware(['api', 'auth:sanctum', 'verified', 'active', '2fa', 'concurrent', 'last_seen'])
    ->group(function () {
        
        Route::post('api/games/baccarat/play', [GameController::class, 'play'])->name('play');
    });
