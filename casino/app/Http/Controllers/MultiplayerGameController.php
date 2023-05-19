<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Http\Controllers;

use App\Helpers\PackageManager;
use App\Http\Requests\GetMultiplayerGame;

class MultiplayerGameController extends Controller
{
    public function index(GetMultiplayerGame $request, $packageId, PackageManager $packageManager)
    {
        $gameServiceClass = $packageManager->getPackageProperty($packageId, 'namespace') . 'Services\\GameService';
        $gameService = (new $gameServiceClass($request->user()))->init();

        return $gameService->getGameActionData('init');
    }
}
