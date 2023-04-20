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

use App\Helpers\Queries\LeaderboardQuery;

class LeaderboardController extends Controller
{
    public function index(LeaderboardQuery $query)
    {
        return [
            'count' => $query->getRowsCount(),
            'items' => $query->get()->makeHidden('email')
        ];
    }
}
