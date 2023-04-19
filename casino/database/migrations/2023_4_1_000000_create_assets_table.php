<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   database/migrations
 * 
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
*/

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateAssetsTable extends Migration
{
    
    public function up()
    {
        Schema::create('assets', function (Blueprint $table) {
            $table->id();
            $table->unsignedTinyInteger('type');
            $table->string('symbol', 50);
            $table->string('name', 180);
            $table->string('external_id', 100);
            $table->unsignedTinyInteger('status');
            $table->unsignedDecimal('price', 20, 8);
            $table->timestamps();
            
            $table->unique(['type', 'symbol']);
            $table->index('name');
            $table->index('status');
            $table->index('external_id');
        });
    }

    
    public function down()
    {
        Schema::dropIfExists('assets');
    }
}
