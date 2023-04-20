<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   DeleteUnusedProvablyFairGames.php
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Console\Commands;

use App\Models\ProvablyFairGame;
use Illuminate\Console\Command;
use Illuminate\Support\Carbon;

class DeleteUnusedProvablyFairGames extends Command
{
    
    protected $signature = 'game:delete-unused';

    
    protected $description = 'Delete unused provably fair games';

    
    public function __construct()
    {
        parent::__construct();
    }

    
    public function handle()
    {
        
        ProvablyFairGame::where('created_at', '<', Carbon::now()->subDay())
            ->doesntHave('game')
            ->delete();

        return 0;
    }
}
