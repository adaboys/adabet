<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   web.php
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */



use Illuminate\Support\Facades\Route;
use App\Http\Controllers\PageController;

Route::get('{path}', [PageController::class, 'index'])->where('path', '.*')->middleware('referrer');
