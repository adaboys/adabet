<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   CallArtisanCommand.php
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Console\Commands;

use Illuminate\Console\Command;
use Illuminate\Support\Facades\Artisan;

class CallArtisanCommand extends Command
{
    
    protected $signature = 'command:call {name=cache:clear}';

    
    protected $description = 'Call artisan command';

    
    public function handle()
    {
        Artisan::call($this->argument('name'));

        return 0;
    }
}
