<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Http\Controllers\Admin;

use App\Helpers\Queries\BonusQuery;
use App\Http\Controllers\Controller;

class BonusController extends Controller
{
    public function index(BonusQuery $query)
    {
        $items = $query
            ->get()
            ->map(function ($game) {
                $game->account->user->makeVisible('referrer');
                $game->account->user->append('two_factor_auth_enabled', 'two_factor_auth_passed', 'is_admin', 'is_bot', 'is_active');
                return $game;
            });

        return [
            'count' => $query->getRowsCount(),
            'items' => $items
        ];
    }
}
